import numpy as np
from scipy.spatial.distance import cdist
from numba import jit, njit, prange


@jit(forceobj=True)
def simulate(boids, perception, asp, coeffs, arange):
    """
    функция для запуска симуляции движения агентов

    :param boids: массив агентов (и их параметров)
    :param perception: на каком расстоянии агенты являютс соседями
    :param asp: характеристика размеров окна
    :param coeffs: коэффициенты взаимодействия
    :param arange: ограничения на ускорение
    :return: None
    """
    D = cdist(boids[:, :2], boids[:, :2])
    M = D < perception
    np.fill_diagonal(M, False)
    wa = wall_avoidance(boids, asp)
    nz = noise(boids)
    for i in prange(boids.shape[0]):
        idx = np.where(M[i])[0]
        accels = np.zeros((5, 2))
        if idx.size > 0:
            accels[0] = alignment(boids, i, idx)
            accels[1] = cohesion(boids, i, idx)
            accels[2] = separation(boids, i, idx, D)
        accels[3] = wa[i]
        accels[4] = nz[i]
        clip_mag(accels, arange[0], arange[1])
        boids[i, 4:6] = np.sum(accels * coeffs.reshape(-1, 1), axis=0)


@njit(cache=True)
def mean_axis0(arr):
    """
    функция заменяющая np.mean

    :param arr: массив для усреднения
    :return: усреднённый массив
    """
    n = arr.shape[1]
    res = np.empty(n, dtype=arr.dtype)
    for i in range(n):
        res[i] = arr[:, i].mean()
    return res


@njit
def clip_mag(arr, low, high):
    """
    функция для применения ограничений на скорости и ускорения

    :param arr: массив
    :param low: минимальное значение
    :param high: максимальное значение
    :return: 'обрезаный' массив
    """
    mag = np.sum(arr * arr, axis=1) ** 0.5
    mask = mag > 1e-16
    mag_cl = np.clip(mag[mask], low, high)
    arr[mask] *= (mag_cl / mag[mask]).reshape(-1, 1)


@njit(cache=True)
def init_boids(boids, asp, vrange=(0., 1.), seed=0):
    """
    функция случайно генерирующая агентов

    :param boids: массив агентов (и их параметров)
    :param asp: характеристика размеров окна
    :param vrange: ограничения на скорости
    :param seed: сид
    :return: None
    """
    N = boids.shape[0]
    np.random.seed(seed)
    boids[:, 0] = np.random.rand(N) * asp
    boids[:, 1] = np.random.rand(N)
    v = np.random.rand(N) * (vrange[1] - vrange[0]) + vrange[0]
    alpha = np.random.rand(N) * 2 * np.pi
    c, s = np.cos(alpha), np.sin(alpha)
    boids[:, 2] = v * c
    boids[:, 3] = v * s


@njit(cache=True)
def directions(boids):
    """
    функяи определяет направление стрелок для отрисовки

    :param boids: массив агентов (и их параметров)
    :return: направления стрелок
    """
    return np.hstack((boids[:, :2] - boids[:, 2:4], boids[:, :2]))


@njit(cache=True)
def propagate(boids, dt, vrange):
    """
    функция расчитывает движение агентов

    :param boids: массив агентов (и их параметров)
    :param dt: параметр дискретизации времени
    :param vrange: ограничения на максимальные и минимальные значения скоростей
    :return: None
    """
    boids[:, :2] += dt * boids[:, 2:4] + 0.5 * dt**2 * boids[:, 4:6]
    boids[:, 2:4] += dt * boids[:, 4:6]
    clip_mag(boids[:, 2:4], vrange[0], vrange[1])


@njit(cache=True)
def periodic_walls(boids, asp):
    """
    функция для расчётов условий периодичности

    :param boids: массив агентов (и их параметров)
    :param asp: характеристика размеров окна
    :return: None
    """
    boids[:, :2] %= np.array([asp, 1.])


@njit(cache=True)
def alignment(boids, i, idx):
    """
    функция расчитывающая выравнивание агентов

    :param boids: массив агентов (и их параметров)
    :param i: индекс текущего агента
    :param idx: индексы соседей текущего агента
    :return: ускорение выравнивания
    """
    return mean_axis0(boids[idx, 2:4]) - boids[i, 2:4]


@njit(cache=True)
def cohesion(boids, i, idx):
    """
    функция расчитывающая притяжение агентов

    :param boids: массив агентов (и их параметров)
    :param i: индекс текущего агента
    :param idx: индексы соседей текущего агента
    :return: ускорение притяжения
    """
    return mean_axis0(boids[idx, 0:2]) - boids[i, 0:2]


@njit(cache=True)
def separation(boids, i, idx, D):
    """
    функция расчитывающая отталкивание агентов

    :param boids: массив агентов (и их параметров)
    :param i: индекс текущего агента
    :param idx: индексы соседей текущего агента
    :param D: матрица расстояний между всеми агентами
    :return: ускорение отталкивания
    """
    return np.sum((boids[i, 0:2] - boids[idx, 0:2]) / D[i][idx].reshape(-1, 1), axis=0)


@njit
def wall_avoidance(boids, asp):
    """
    функция расчитывающая отталкивание стен

    :param boids: массив агентов (и их параметров)
    :param asp: характеристика размеров окна
    :return: ускорение отталкивания стен
    """
    left = np.abs(boids[:, 0])
    right = np.abs(asp - boids[:, 0])
    bottom = np.abs(boids[:, 1])
    top = np.abs(1 - boids[:, 1])

    ax = 1. / left**2 - 1. / right**2
    ay = 1. / bottom**2 - 1. / top**2

    return np.column_stack((ax, ay))


@njit(cache=True)
def noise(boids):
    """
    функция вносящая шум в движение агентов

    :param boids: массив агентов (и их параметров)
    :return: шумовое ускорение
    """
    return np.random.rand(boids.shape[0], 2) * 2 - 1