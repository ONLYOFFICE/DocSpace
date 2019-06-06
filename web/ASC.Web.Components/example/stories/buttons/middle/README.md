# Buttons: Button

## Usage

```js
import { Button } from 'asc-web-components';
```

#### Description

Middle Button is used for a action on a page.

#### Usage

```js
<Button size='middle' isDisabled={false} onClick={() => alert('Button clicked')} label="OK" />
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `primary`          | `bool`   |    -     | -                           | -         | Tells when the button should be primary                                  |
| `isDisabled`         | `bool`   |    -     | -                           | -         | Tells when the button should present a disabled state                                  |
| `onClick`          | `func`   |    ✅    | -                           | -         | What the button will trigger when clicked                                              |
| `size`             | `oneOf`  |    -     | `base`, `middle`, `big`, `huge`             | `base`     | Size of button      |
| `label`             | `string`  |    -     | -             | -     | Button text                     |

