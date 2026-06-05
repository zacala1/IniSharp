# Changelog

모든 주요 변경사항을 기록합니다.

## [Unreleased] - 2026-06-01

### 수정
- `;`/`#` 주석 prefix 보존 및 `SaveOptions.NormalizeCommentPrefix` 동작 문서화
- `:` 키/값 구분자 로드 지원
- 섹션 헤더 뒤 잘못된 trailing content를 파싱 에러로 수집하고 해당 섹션 줄을 건너뛰도록 수정
- `Document.GetSections()`와 `Section.GetProperties()`가 내부 리스트를 직접 노출하지 않도록 수정
- 섹션 이름의 대괄호와 속성 이름의 `=` 검증 강화
- `DateTimeOffset` 및 `Nullable<DateTimeOffset>` 역직렬화 지원
- 배열 변환에서 빈 문자열, bool alias, enum 변환 처리 개선
- 스키마 range 검증에서 숫자가 아닌 값은 `TypeMismatch`로 보고
- XML export 주석 sanitize 처리 추가

### 개선
- `SaveAsync(Stream...)`가 문서 전체를 `MemoryStream`에 한 번 더 버퍼링하지 않고 대상 스트림에 직접 쓰도록 최적화
- GUI dirty tracking을 undo stack 개수 기반에서 save-point 기반으로 변경
- GUI Save/Save As 실패 시 파일 경로/상태 오염 방지
- GUI UTF-32 BOM 인코딩 감지 지원
- GUI TreeView/ListView 선택 동기화, 필터 상태 reordering, Replace All undo, 선택 merge dirty 처리 개선
- 텍스트 편집 중 Ctrl+C/X/V/Z/Y/A 및 Delete 단축키가 에디터 명령으로 가로채지 않도록 수정

### 문서
- README와 `docs/`의 저장 API, async API, 직렬화 타입, 파싱/저장 옵션 예제를 최신 API에 맞게 갱신
- 잘못된 `Save(stream, doc)`, `TextWriter` 저장 예제 및 `IniSharp.Serialization` using 예제 제거

## [0.1.0] - 2024-12-11

### 추가
- `MaxSections` - 최대 섹션 수 제한 옵션
- `MaxPropertiesPerSection` - 섹션당 최대 속성 수 제한
- `MaxValueLength` - 값 최대 길이 제한
- `MaxLineLength` - 줄 최대 길이 제한 (메모리 소진 공격 방지)
- `Document.DefaultSectionName` 상수 추가
- `Save_NullDocument_ThrowsArgumentNullException` 테스트

### 변경
- `Save` 메서드가 `IsQuoted` 속성을 변경하지 않도록 수정
- `LoadWithOptionsAsync`를 진정한 async I/O로 변환
- `HasSection`/`HasProperty`가 빈 문자열에 `ArgumentException` 발생
- `DocumentSnapshot`이 `Stack` 대신 `LinkedList` 사용 (O(1) trim)
- `MergeFromOnLastWin` 성능 개선 O(n*m) → O(n+m)
- 파라미터 이름 일관성 개선 (`registry` → `document`)

### 보안
- `FilteringExtensions`의 Regex에 100ms 타임아웃 추가 (ReDoS 방지)
- `Save` 메서드에 document null 체크 추가
- `Comment.Prefix` 검증 추가 (단일 문자만 허용)

### 문서
- README를 자연스러운 한국어로 재작성
- `docs/` 폴더 구조화
  - `configuration.md` - 설정 옵션 문서
  - `advanced.md` - 고급 기능 문서
  - `api-reference.md` - API 레퍼런스
- `FEATURES.md` 삭제

## [0.0.1] - 2024-12-10

### 추가
- 최초 릴리스
- INI 파일 파싱 및 저장
- 섹션, 속성, 주석 지원
- 중복 섹션/키 처리 정책
- 비동기 API 지원
- 환경변수 치환
- 스냅샷 및 Undo 기능
- 문서 비교 (Diff)
- Fluent Builder API
