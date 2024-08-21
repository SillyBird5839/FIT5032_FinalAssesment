from flask import Flask, request, Response
import requests
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # 启用CORS

@app.route('/proxy', methods=['POST'])
def proxy():
    data = request.json
    headers = {
        "Authorization": "Bearer hJzyhmhYSaUpFJWWEWTp:gArlHKqhCACZPFsMdFvc",
        "Content-Type": "application/json"
    }

    # 发送请求到外部API，并以流式方式处理响应
    response = requests.post("https://spark-api-open.xf-yun.com/v1/chat/completions", json=data, headers=headers, stream=True)

    full_data = ""
    try:
        # 遍历响应流中的每一行
        for line in response.iter_lines():
            if line:
                decoded_line = line.decode('utf-8')
                full_data += decoded_line + "\n"  # 加入换行符保持原始格式
                if "data: [DONE]" in decoded_line:  # 检测到完成标记
                    break
    except Exception as e:
        print("错误:", e)
        return Response("错误处理请求", status=500)

    # 返回所有收集到的数据
    return Response(full_data, mimetype='text/plain')

if __name__ == '__main__':
    app.run(port=5000, debug=True, ssl_context=('cert.pem', 'key.pem'))
