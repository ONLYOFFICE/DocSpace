# SocialButton

Button is used for sign up with help social networks

### Usage

```js
import { SocialButton } from "asc-web-components";
```

```jsx
<SocialButton
  iconName={"SocialButtonGoogleIcon"}
  label={"Sign up with Google"}
/>
```

### Properties

| Props        |   Type   | Required | Values |         Default          | Description                                           |
| ------------ | :------: | :------: | :----: | :----------------------: | ----------------------------------------------------- |
| `label`      | `string` |    -     |   -    |            -             | Button text                                           |
| `iconName`   | `string` |    -     |   -    | `SocialButtonGoogleIcon` | Icon of button                                        |
| `isDisabled` |  `bool`  |    -     |   -    |         `false`          | Tells when the button should present a disabled state |
| `onClick`    |  `func`  |    -     |   -    |            -             | What the button will trigger when clicked             |
