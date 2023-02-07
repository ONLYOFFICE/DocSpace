# PasswordInput

Password entry field with advanced capabilities for displaying, validation of correspondence and generation based on settings

### Usage

```js
import PasswordInput from "@docspace/components/password-input";
```

```js
const settings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false,
};
```

```jsx
<PasswordInput
  inputName="demoPasswordInput"
  emailInputName="demoEmailInput"
  inputValue={value}
  onChange={(e) => {
    set(e.target.value);
  }}
  clipActionResource="Copy e-mail and password"
  clipEmailResource="E-mail: "
  clipPasswordResource="Password: "
  tooltipPasswordTitle="Password must contain:"
  tooltipPasswordLength="from 6 to 30 characters"
  tooltipPasswordDigits="digits"
  tooltipPasswordCapital="capital letters"
  tooltipPasswordSpecial="special characters (!@#$%^&*)"
  generatorSpecial="!@#$%^&*"
  passwordSettings={settings}
  isDisabled={false}
  placeholder="password"
  onValidateInput={(a) => console.log(a)}
  onCopyToClipboard={(b) => console.log("Data " + b + " copied to clipboard")}
/>
```

Object with settings:

```js
{
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false
}
```

Check for compliance with settings is carried out on fly. As you type in required number of characters, progress bar will fill up and when all conditions are met, the color will change from red to green.

Depending on screen width of device, input will change location of elements.

When setting focus to input, tooltip will be shown with progress in fulfilling conditions specified in settings. When unfocused, tooltip disappears.

You can apply all the parameters of the InputBlock component to the component.

When button is pressed, copy data will be copied to clipboard and copy action will be blocked for 2 seconds. In future, the button is unlocked.

If emailInputName parameter value is empty, copy action will be disabled.

### Properties

| Props                    | Type           | Required |       Values       |     Default     | Description                                                                                                                            |
| ------------------------ | :------------- | :------: | :----------------: | :-------------: | -------------------------------------------------------------------------------------------------------------------------------------- |
| `autoComplete`           | `string`       |    -     |         -          | `new-password`  | Allows you to set the component auto-complete                                                                                          |
| `className`              | `string`       |    -     |         -          |        -        | Accepts class                                                                                                                          |
| `clipActionResource`     | `string`       |    -     |         -          |        -        | Translation of text for copying email data and password                                                                                |
| `clipCopiedResource`     | `string`       |    -     |         -          |    `Copied`     | Text translation copy action to copy                                                                                                   |
| `clipEmailResource`      | `string`       |    -     |         -          |    `E-mail`     | Text translation email to copy                                                                                                         |
| `clipPasswordResource`   | `string`       |    -     |         -          |   `Password`    | Text translation password to copy                                                                                                      |
| `emailInputName`         | `string`       |    ✅    |         -          |        -        | Required to associate password field with email field                                                                                  |
| `generatorSpecial`       | `string`       |    -     |         -          |   `!@#$%^&*`    | Set of special characters for password generator and validator                                                                         |
| `id`                     | `string`       |    -     |         -          |        -        | Allows you to set the component id                                                                                                     |
| `inputName`              | `string`       |    -     |         -          | `passwordInput` | Input name                                                                                                                             |
| `inputType`              | `array`        |    -     | `text`, `password` |   `password`    | It is necessary for correct display of values ​​inside input                                                                           |
| `inputValue`             | `string`       |    -     |         -          |        -        | Input value                                                                                                                            |
| `inputWidth`             | `string`       |    -     |         -          |        -        | If you need to set input width manually                                                                                                |
| `isDisabled`             | `bool`         |    -     |         -          |     `false`     | Set input disabled                                                                                                                     |
| `onChange`               | `func`         |    -     |         -          |        -        | Will be triggered whenever an PasswordInput typing                                                                                     |
| `onCopyToClipboard`      | `func`         |    -     |         -          |        -        | Will be triggered if you press copy button, return formatted value                                                                     |
| `onValidateInput`        | `func`         |    -     |         -          |        -        | Will be triggered whenever an PasswordInput typing, return bool value                                                                  |
| `passwordSettings`       | `object`       |    ✅    |         -          |        -        | Set of settings for password generator and validator                                                                                   |
| `simpleView`             | `bool`         |    -     |         -          |     `false`     | Set simple view of password input (without tooltips, password progress bar and several additional buttons (copy and generate password) |
| `style`                  | `obj`, `array` |    -     |         -          |        -        | Accepts css style                                                                                                                      |
| `tooltipPasswordCapital` | `string`       |    -     |         -          |        -        | Capital text translation tooltip                                                                                                       |
| `tooltipPasswordDigits`  | `string`       |    -     |         -          |        -        | Digit text translation tooltip                                                                                                         |
| `tooltipPasswordLength`  | `string`       |    -     |         -          |        -        | Password text translation is long tooltip                                                                                              |
| `tooltipPasswordSpecial` | `string`       |    -     |         -          |        -        | Special text translation tooltip                                                                                                       |
| `tooltipPasswordTitle`   | `string`       |    -     |         -          |        -        | Text translation tooltip                                                                                                               |
| `hideNewPasswordButton`  | `bool`         |    -     |         -          |     `false`     | Allows to hide NewPasswordButton                                                                                                       |
| `isDisableTooltip`       | `bool`         |    -     |         -          |     `false`     | Allows to hide Tooltip                                                                                                                 |
| `isTextTooltipVisible`   | `bool`         |    -     |         -          |     `false`     | Allows to show text Tooltip                                                                                                            |

#### passwordSettings properties

| Props         |   Type   | Required | Values | Default | Description                     |
| ------------- | :------: | :------: | :----: | :-----: | ------------------------------- |
| `digits`      |  `bool`  |    ✅    |   -    |    -    | Must contain digits             |
| `minLength`   | `number` |    ✅    |   -    |    -    | Minimum password length         |
| `specSymbols` |  `bool`  |    ✅    |   -    |    -    | Must contain special characters |
| `upperCase`   |  `bool`  |    ✅    |   -    |    -    | Must contain uppercase letters  |
