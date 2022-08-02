# Room logo

Tag allow you display virtual room tag

### Usage

```js
import Tag from "@docspace/components/tag";
```

```jsx
<Tag
  tag="tag"
  label={"script"}
  isNewTag={false}
  isDisabled={false}
  onDelete={}
  onClick={}
  advancedOptions={}
  tagMaxWidth={}
/>
```

| Props            |      Type      | Required | Values | Default | Description                                                     |
| ---------------- | :------------: | :------: | :----: | :-----: | --------------------------------------------------------------- |
| `id`             |    `string`    |    -     |   -    |    -    | Accepts id                                                      |
| `className`      |    `string`    |    -     |   -    |    -    | Accepts class                                                   |
| `style`          | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                               |
| `tag`            |    `string`    |    -     |   -    |    -    | Accepts tag id                                                  |
| `label`          |    `string`    |    -     |   -    |    -    | Accepts the tag label                                           |
| `isNewTag`       |   `boolean`    |    -     |   -    | `false` | Accepts the tag styles as new and add delete button             |
| `isDisabled`     |   `boolean`    |    -     |   -    | `false` | Accepts the tag styles as disabled and disable click            |
| `onClick`        |   `function`   |    -     |   -    |    -    | Accepts the function that called when tag clicked               |
| `onDelete`       |   `function`   |    -     |   -    |    -    | Accepts the function that called when tag delete button clicked |
| `tagMaxWidth`    |    `string`    |    -     |   -    |    -    | Accepts the max width of tag                                    |
| `advancedOption` |    `object`    |    -     |   -    |    -    | Accepts the dropdowns options                                   |
