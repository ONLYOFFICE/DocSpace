# Buttons: Button

## Usage

```js
import { Button } from 'asc-web-components';
```

#### Description

Buttons are used for a action on a page.

#### Usage

```js
<Button primary={true} size='big' disabled={true} onClick={() => alert('Button clicked')}>Save message</Button>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `primary`          | `bool`   |    -     | -                           | -         | Tells when the button should be primary                                  |
| `isDisabled`         | `bool`   |    -     | -                           | -         | Tells when the button should present a disabled state                                  |
| `onClick`          | `func`   |    ✅    | -                           | -         | What the button will trigger when clicked                                              |
| `size`             | `oneOf`  |    -     | `base`, `middle`, `big`, `huge`             | `base`     | -                                                                     |

The component further forwards all `data-` and `aria-` attributes to the underlying `button` component.

Main Functions and use cases are:

- Primary action _example: Save changes_

- Not primary action _example: Cancel_

- Affirming affects _example: Submit a form_

- Attracting attention _example: Add a discount rule_