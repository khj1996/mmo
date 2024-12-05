using System;

namespace ServerCore
{
    public class RecvBuffer
    {
        //버퍼
        ArraySegment<byte> _buffer;

        //읽기 시작 위치
        int _readPos;

        //쓰기 시작 위치
        int _writePos;


        //버퍼 생성자
        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        //현재 입력된 데이터 크기
        public int DataSize => _writePos - _readPos;

        //입력 가능한 버퍼 크기
        public int FreeSize => _buffer.Count - _writePos;

        //읽는 부분 반환
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        //쓰는 부분 반환
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        //버퍼 재사용을 위한 데이터 초기화
        public void Clean()
        {
            int dataSize = DataSize;
            
            // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
            if (dataSize == 0)
            {
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 찌끄레기가 있으면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        //읽기
        public bool OnRead(int numOfBytes)
        {
            //읽는 크기가 데이터 사이즈보다 크면 실패처리
            if (numOfBytes > DataSize)
                return false;

            //처리한 크기만큼 현재 읽는 부분 증가
            _readPos += numOfBytes;
            return true;
        }

        //쓰기
        public bool OnWrite(int numOfBytes)
        {
            //쓰는 크기가 여유 사이즈보다 크면 실패처리
            if (numOfBytes > FreeSize)
                return false;

            //작성한 크기만큼 쓰는 위치 수정
            _writePos += numOfBytes;
            return true;
        }
    }
}