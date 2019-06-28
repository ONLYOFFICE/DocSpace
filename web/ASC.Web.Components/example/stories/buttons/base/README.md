# Buttons: Button

## Usage

```js
import { Button } from 'asc-web-components';
```

#### Description

Button is used for a action on a page.

#### Usage

```js
<Button size='base' isDisabled={false} onClick={() => alert('Button clicked')} label="OK" />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `label`             | `string`  |    -     | -             | -     | Button text                     |
| `primary`          | `bool`   |    -     | -                           | -         | Tells when the button should be primary                                  |
| `size`             | `oneOf`  |    -     | `base`, `middle`, `big`, `huge`             | `base`     | Size of button      |
| `isDisabled`         | `bool`   |    -     | -                           | -         | Tells when the button should present a disabled state                                  |
| `isLoading`             | `bool`  |    -     | -             | -     | Tells when the button should show loader icon                      |
| `onClick`          | `func`   |    ✅    | -                           | -         | What the button will trigger when clicked                                              |

