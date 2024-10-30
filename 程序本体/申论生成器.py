import json
import random
import argparse
import time  # 导入 time 模块
import re  # 导入 re 模块

# 设置随机种子，以确保每次运行时生成的内容有所不同
random.seed(time.time())

class Slscq:
    def __init__(self, json_path):
        """
        初始化类实例，加载数据源文件。
        
        :param json_path: 数据源 JSON 文件的路径
        """
        with open(json_path, 'r', encoding='utf-8') as file:
            self.data = json.load(file)  # 将 JSON 文件内容加载到 self.data 中

    def get_random_element(self, element_type: str) -> str:
        """
        从指定类型的元素列表中随机选择一个元素。
        
        :param element_type: 元素类型，如 'verb'、'noun' 等
        :return: 随机选择的元素
        """
        return random.choice(self.data[element_type])

    def replace_xx(self, input_str: str, theme: str) -> str:
        """
        替换字符串中的 'xx' 为指定的主题。
        
        :param input_str: 输入字符串
        :param theme: 文章主题
        :return: 替换后的字符串
        """
        return input_str.replace('xx', theme)

    def replace_vn(self, input_str: str) -> str:
        """
        替换字符串中的 'vn' 为随机动词和名词的组合。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(
            r'vn', 
            lambda x: '，'.join([self.get_random_element('verb') + self.get_random_element('noun') for _ in range(random.randint(1, 4))]), 
            input_str
        )

    def replace_v(self, input_str: str) -> str:
        """
        替换字符串中的 'v' 为随机动词。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(r'v', lambda x: self.get_random_element('verb'), input_str)

    def replace_n(self, input_str: str) -> str:
        """
        替换字符串中的 'n' 为随机名词。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(r'n', lambda x: self.get_random_element('noun'), input_str)

    def replace_ss(self, input_str: str) -> str:
        """
        替换字符串中的 'ss' 为随机句子。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(r'ss', lambda x: self.get_random_element('sentence'), input_str)

    def replace_sp(self, input_str: str) -> str:
        """
        替换字符串中的 'sp' 为随机并列句。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(r'sp', lambda x: self.get_random_element('parallel_sentence'), input_str)

    def replace_p(self, input_str: str) -> str:
        """
        替换字符串中的 'p' 为随机短语。
        
        :param input_str: 输入字符串
        :return: 替换后的字符串
        """
        return re.sub(r'p', lambda x: self.get_random_element('phrase'), input_str)

    def replace_all(self, input_str: str, theme: str) -> str:
        """
        替换字符串中的所有占位符。
        
        :param input_str: 输入字符串
        :param theme: 文章主题
        :return: 替换后的字符串
        """
        input_str = self.replace_vn(input_str)
        input_str = self.replace_v(input_str)
        input_str = self.replace_n(input_str)
        input_str = self.replace_ss(input_str)
        input_str = self.replace_sp(input_str)
        input_str = self.replace_p(input_str)
        input_str = self.replace_xx(input_str, theme)
        return input_str

    def gen(self, theme: str = '年轻人买房', essay_num: int = 500) -> dict:
        """
        生成一篇文章。
        
        :param theme: 文章主题
        :param essay_num: 文章最少字数
        :return: 包含标题、开头、正文和结尾的字典
        """
        end_num = begin_num = int(essay_num * 0.15)  # 计算开头和结尾的字数
        body_num = int(essay_num * 0.7)  # 计算正文的字数

        title = self.replace_all(self.get_random_element('title'), theme)  # 生成标题
        begin = ''  # 初始化开头
        body = ''  # 初始化正文
        end = ''  # 初始化结尾

        # 生成开头部分
        while len(begin) < begin_num:
            begin += self.replace_all(self.get_random_element('beginning'), theme)

        # 生成正文部分
        while len(body) < body_num:
            body += self.replace_all(self.get_random_element('body'), theme)

        # 生成结尾部分
        while len(end) < end_num:
            end += self.replace_all(self.get_random_element('ending'), theme)

        return {'title': title, 'begin': begin, 'body': body, 'end': end}

    def gen_text(self, theme: str = '年轻人买房', essay_num: int = 500) -> str:
        """
        生成并返回完整的文章文本。
        
        :param theme: 文章主题
        :param essay_num: 文章最少字数
        :return: 完整的文章文本
        """
        result = self.gen(theme, essay_num)
        return f"{result['title']}\n    {result['begin']}\n    {result['body']}\n    {result['end']}"

if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        prog='slscq.py',
        description='自动生成一篇垃圾文章'
    )
    parser.add_argument('theme', help='文章主题 (例如: 年轻人买房)', type=str, nargs='?', default='年轻人买房')
    parser.add_argument('-n', '--essay_num', help='文章最少字数 (默认 500)', type=int, default=500, metavar='num')
    parser.add_argument('-d', '--data_source', help='数据源 JSON 文件路径 (默认 ../data.json)', type=str, default='../data.json', metavar='json file')
    args = parser.parse_args()

    # 交互式输入
    if args.theme == '年轻人买房':
        args.theme = input("请输入文章主题 (默认: 年轻人买房): ") or '年轻人买房'
    if args.essay_num == 500:
        args.essay_num = int(input("请输入文章最少字数 (默认: 500): ") or 500)

    try:
        arc_gen = Slscq(args.data_source)  # 创建 Slscq 类的实例
        arc_text = arc_gen.gen_text(args.theme, args.essay_num)  # 生成文章
        actual_essay_num = len(arc_text)  # 计算实际文章字数
        print(f"生成的文章主题: {args.theme}")
        print(f"生成的文章字数: {actual_essay_num}")
        print("\n生成的文章:")
        print(arc_text)
    except FileNotFoundError:
        print(f"错误: 数据源文件 {args.data_source} 未找到")
    except json.JSONDecodeError:
        print(f"错误: 数据源文件 {args.data_source} 格式不正确")
