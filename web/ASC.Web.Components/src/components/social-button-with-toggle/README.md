# Buttons: SocialButtonWithToggle

## Usage

```js
import { SocialButtonWithToggle } from 'asc-web-components';

```

#### Description

Button is used for sign up with help social networks, with toggle for fast connecting or disconnecting from them.   

#### Usage

```js
<SocialButtonWithToggle iconName={"ShareFacebookIcon"} label={"Facebook"}/>
```

#### Properties

| Props              | Type     | Required | Values                      | Default   | Description                                                                                                                                      |
| ------------------ | -------- | :------: | --------------------------- | --------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| `label`             | `string`  |    -     | -             | -     | Button text                     |
| `iconName`          | `string`   |    -     | -                           | ShareFacebookIcon         | Icon of button                                  |
| `isDisabled`         | `bool`   |    -     | -                           | false         | Tells when the button should present a disabled state                                  |
| `IsChecked`          | `bool`   |    -    | -                           | false         | Is connected a social network?                                              |
| `onClick`          | `func`   |    -    | -                           | -         | What the button will trigger when clicked                                              |


