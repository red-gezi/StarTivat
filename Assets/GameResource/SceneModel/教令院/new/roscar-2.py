from flask import Flask, request, jsonify

app = Flask(__name__)

class TwistCommand:
    def __init__(self, x, y, z, th, speed, turn):
        self.x = x
        self.y = y
        self.z = z
        self.th = th
        self.speed = speed
        self.turn = turn

@app.route('/cmd/', methods=['POST'])
def receive_cmd():
    data = request.get_json()
    command = TwistCommand(
        x=data['x'],
        y=data['y'],
        z=data['z'],
        th=data['th'],
        speed=data['speed'],
        turn=data['turn']
    )
    print(command.x, command.y, command.z, command.th, command.speed, command.turn)
    #pub_thread.update(command.x, command.y, command.z, command.th, command.speed, command.turn)
    return jsonify({"message": "Data received and processed"})

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8000, debug=True, use_reloader=False)