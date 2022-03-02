import numpy as np
from vispy import app, scene
from funcs import *
import ffmpeg
from datetime import datetime
app.use_app('pyglet')


asp = 16 / 9
h = 720
w = int(h * asp)

canvas = scene.SceneCanvas(keys='interactive',
                           show=True,
                           size=(w, h))

view = canvas.central_widget.add_view()
view.camera = scene.PanZoomCamera(rect=(0, 0, asp, 1),
                                  aspect=1)


video = True
N = 10000
dt = 0.05
perception = 1 / 20
vrange = np.array([0.05, 0.1])
arange = np.array([0.0, 1.0])

coeffs = np.array([2.0,       # alignment
                   0.75,       # cohesion
                   0.05,      # separation
                   0.00001,     # walls
                   0.001])     # noise


boids = np.zeros((N, 6), dtype=np.float64)
D = np.zeros((N, N), dtype=np.float64)
init_boids(boids, asp, vrange)


arrows = scene.Arrow(arrows=directions(boids),
                     arrow_color=(1, 1, 1, 1),
                     arrow_size=4,
                     connect='segments',
                     parent=view.scene)

scene.Line(pos=np.array([[0, 0],
                         [asp, 0],
                         [asp, 1],
                         [0, 1],
                         [0, 0]
                         ]),
           color=(1, 0, 0, 1),
           connect='strip',
           method='gl',
           parent=view.scene
           )


scene.visuals.Text(text='N = ' + str(N) + '\n' +
                        'a = ' + str(coeffs[0]) + '\n' +
                        'b = ' + str(coeffs[1]) + '\n' +
                        'c = ' + str(coeffs[2]) + '\n' +
                        'd = ' + str(coeffs[3]) + '\n' +
                        'e = ' + str(coeffs[4]),
                   pos=[0.08, 0.9],
                   font_size=13,
                   color=(1, 0, 0, 1),
                   face='Times New Roman',
                   parent=view.scene)


fps_value = 0


def fps_update(cur_fps):
    """
    Функция, которая обновляет значение глобальной переменной fps_value до актуального значения fps
    :param cur_fps: текущее состояние fps
    :return: None
    """
    global fps_value
    fps_value = cur_fps
    print(f'{fps_value:.1f} FPS')


fps_text = scene.Text(f'fps: {fps_value:.1f}',
                   pos=[0.08, 0.05],
                   font_size=15,
                   color=(1, 0, 0, 1),
                   face='Times New Roman',
                   parent=view.scene)


if video:
    fname = f"boids_{datetime.now().strftime('%Y%m%d_%H%M%S')}.mp4"


    process = (
        ffmpeg
            .input('pipe:', format='rawvideo', pix_fmt='rgb24', s=f'{w}x{h}', r=60)
            .output(fname, pix_fmt='yuv420p', preset='slower', r=60)
            .overwrite_output()
            .run_async(pipe_stdin=True)
    )


def update(event):
    """
    функция обновляющая кадры

    :param event: переменная уведомляет о том, что нужно пересчитать кадр
    :return: None
    """
    global process, boids, D
    simulate(boids, perception, asp, coeffs, arange)
    propagate(boids, dt, vrange)
    periodic_walls(boids, asp)
    arrows.set_data(arrows=directions(boids))
    fps_text.text = f'fps: {fps_value:.1f}'
    if video:
        frame = canvas.render(alpha=False)
        process.stdin.write(frame.tobytes())
    else:
        canvas.update(event)
    print(f"{canvas.fps:0.1f}")


timer = app.Timer(interval=0,
                  start=True,
                  connect=update)


if __name__ == '__main__':
    canvas.measure_fps(callback=fps_update)
    app.run()
    if video:
        process.stdin.close()
        process.wait()