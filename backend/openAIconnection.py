def initialise():
    from openai import OpenAI
    import os
    client = OpenAI(api_key=os.environ["OPENAI_API_KEY"])

    return client

def query(client, message):
    completion = client.chat.completions.create(
        model="gpt-3.5-turbo",
        messages=[
            {"role": "system", "content": "You are an assistant."},
            {"role": "user", "content": str(message)}
        ]
    )
    return completion.choices[0].message.content