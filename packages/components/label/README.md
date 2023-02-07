# Label

Component displays the field name in the form

### Usage

```js
import Label from "@docspace/components/label";
```

```jsx
<Label
  text="First name:"
  title="first name"
  htmlFor="firstNameField"
  display="block"
/>
```

### Properties

| Props        |      Type      | Required | Values | Default | Description                                                                 |
| ------------ | :------------: | :------: | :----: | :-----: | --------------------------------------------------------------------------- |
| `className`  |    `string`    |    -     |   -    |    -    | Class name                                                                  |
| `display`    |    `string`    |    -     |   -    |    -    | Sets the 'display' property                                                 |
| `error`      |     `bool`     |    -     |   -    |    -    | Indicates that the field to which the label is attached is incorrect        |
| `htmlFor`    |    `string`    |    -     |   -    |    -    | The field ID to which the label is attached                                 |
| `id`         |    `string`    |    -     |   -    |    -    | Accepts id                                                                  |
| `isInline`   |     `bool`     |    -     |   -    | `false` | Sets the 'display: inline-block' property                                   |
| `isRequired` |     `bool`     |    -     |   -    | `false` | Indicates that the field to which the label is attached is required to fill |
| `style`      | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                           |
| `text`       |    `string`    |    -     |   -    |    -    | Text                                                                        |
| `title`      |    `string`    |    -     |   -    |    -    | Title                                                                       |
| `truncate`   |     `bool`     |    -     |   -    | `false` | Disables word wrapping                                                      |
