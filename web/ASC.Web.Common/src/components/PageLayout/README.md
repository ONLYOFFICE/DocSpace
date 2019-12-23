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

| Props               |  Type  | Required | Values | Default | Description                               |
| ------------------- | :----: | :------: | :----: | :-----: | ----------------------------------------- |
| `isBackdropVisible` | `bool` |    -     |   -    | `false` | If you need display Backdrop              |
| `isNavHoverEnabled` | `bool` |    -     |   -    | `true`  | If you need hover navigation on Backdrop  |
| `isNavOpened`       | `bool` |    -     |   -    | `false` | If you need display navigation            |
| `isAsideVisible`    | `bool` |    -     |   -    | `false` | If you need display aside                 |
| `withBodyScroll`    | `bool` |    -     |   -    | `true`  | If you need display scroll inside content |