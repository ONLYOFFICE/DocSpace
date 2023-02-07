# SocialButton

Button is used for sign up with help social networks

### Usage

```js
import SocialButton from "@docspace/components/social-button";
```

```jsx
<SocialButton
  iconName={"SocialButtonGoogleIcon"}
  label={"Sign up with Google"}
/>
```

### Properties

| Props        |      Type      | Required | Values |         Default          | Description                                           |
| ------------ | :------------: | :------: | :----: | :----------------------: | ----------------------------------------------------- |
| `className`  |    `string`    |    -     |   -    |            -             | Accepts class                                         |
| `iconName`   |    `string`    |    -     |   -    | `SocialButtonGoogleIcon` | Icon of button                                        |
| `id`         |    `string`    |    -     |   -    |            -             | Accepts id                                            |
| `isDisabled` |     `bool`     |    -     |   -    |         `false`          | Tells when the button should present a disabled state |
| `label`      |    `string`    |    -     |   -    |            -             | Button text                                           |
| `onClick`    |     `func`     |    -     |   -    |            -             | What the button will trigger when clicked             |
| `style`      | `obj`, `array` |    -     |   -    |            -             | Accepts css style                                     |
