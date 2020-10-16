# PageLayout

Default page layout

### Usage

```js
import { PageLayout } from "asc-web-common";
```

```jsx
<PageLayout
  articleHeaderContent={articleHeaderContent}
  articleMainButtonContent={articleMainButtonContent}
  articleBodyContent={articleBodyContent}
  sectionHeaderContent={sectionHeaderContent}
  sectionFilterContent={sectionFilterContent}
  sectionBodyContent={sectionBodyContent}
  sectionPagingContent={sectionPagingContent}
/>
```

### Properties

| Props                      |  Type  | Required | Values | Default | Description                               |
| -------------------------- | :----: | :------: | :----: | :-----: | ----------------------------------------- |
| `articleHeaderContent`     | `bool` |    -     |   -    |    -    | Article header content                    |
| `articleMainButtonContent` | `bool` |    -     |   -    |    -    | Article main button content               |
| `articleBodyContent`       | `bool` |    -     |   -    |    -    | Article body content                      |
| `sectionHeaderContent`     | `bool` |    -     |   -    |    -    | Section header content                    |
| `sectionFilterContent`     | `bool` |    -     |   -    |    -    | Section filter content                    |
| `sectionBodyContent`       | `bool` |    -     |   -    |    -    | Section body content                      |
| `sectionPagingContent`     | `bool` |    -     |   -    |    -    | Section paging content                    |
| `withBodyScroll`           | `bool` |    -     |   -    | `true`  | If you need display scroll inside content |
| `withBodyAutoFocus`        | `bool` |    -     |   -    | `false` | If you need set focus on content element  |
