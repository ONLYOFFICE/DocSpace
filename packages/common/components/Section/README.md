# Section

Default section

### Usage

```js
import Section from "@docspace/common/components/Section";
```

```jsx
<Section withBodyScroll={true}>
  <Section.SectionHeader>{sectionHeaderContent}</Section.SectionHeader>

  <Section.SectionFilter>{sectionFilterContent}</Section.SectionFilter>

  <Section.SectionBody>{sectionBodyContent}</Section.SectionBody>

  <Section.SectionPaging>{sectionPagingContent}</Section.SectionPaging>
</Section>
```

### Properties

| Props                  |  Type  | Required | Values | Default | Description                               |
| ---------------------- | :----: | :------: | :----: | :-----: | ----------------------------------------- |
| `sectionHeaderContent` | `bool` |    -     |   -    |    -    | Section header content                    |
| `sectionFilterContent` | `bool` |    -     |   -    |    -    | Section filter content                    |
| `sectionBodyContent`   | `bool` |    -     |   -    |    -    | Section body content                      |
| `sectionPagingContent` | `bool` |    -     |   -    |    -    | Section paging content                    |
| `withBodyScroll`       | `bool` |    -     |   -    | `true`  | If you need display scroll inside content |
| `withBodyAutoFocus`    | `bool` |    -     |   -    | `false` | If you need set focus on content element  |
