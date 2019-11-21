# Label

Component displays the field name in the form

### Usage

```js
import { Label } from "asc-web-components";
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

| Props        |   Type   | Required | Values | Default | Description                                                                 |
| ------------ | :------: | :------: | :----: | :-----: | --------------------------------------------------------------------------- |
| `isRequired` |  `bool`  |    -     |   -    | `false` | Indicates that the field to which the label is attached is required to fill |
| `error`      |  `bool`  |    -     |   -    |    -    | Indicates that the field to which the label is attached is incorrect        |
| `isInline`   |  `bool`  |    -     |   -    | `false` | Sets the 'display: inline-block' property                                   |
| `display`    | `string` |    -     |   -    |    -    | Sets the 'display' property                                                 |
| `title`      | `string` |    -     |   -    |    -    | Title                                                                       |
| `truncate`   |  `bool`  |    -     |   -    | `false` | Disables word wrapping                                                      |
| `htmlFor`    | `string` |    -     |   -    |    -    | The field ID to which the label is attached                                 |
| `text`       | `string` |    -     |   -    |    -    | Text                                                                        |
| `className`  | `string` |    -     |   -    |    -    | Class name                                                                  |
