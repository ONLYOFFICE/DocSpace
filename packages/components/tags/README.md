# Room logo

Tags allow you display virtual room tags

### Usage

```js
import Tags from "@docspace/components/tags";
```

```jsx
<Tags
  tag=["tag1","tag2"]
  onSelectTag={}
/>
```

| Props         |      Type      | Required | Values | Default | Description                                        |
| ------------- | :------------: | :------: | :----: | :-----: | -------------------------------------------------- |
| `id`          |    `string`    |    -     |   -    |    -    | Accepts id                                         |
| `className`   |    `string`    |    -     |   -    |    -    | Accepts class                                      |
| `style`       | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                  |
| `tags`        |    `array`     |    -     |   -    |    -    | Accepts tags                                       |
| `onSelectTag` |   `function`   |    -     |   -    |    -    | Accepts the function that called when tag selected |
