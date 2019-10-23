# PasswordInput

#### Description

Password entry field with advanced capabilities for displaying, validation of correspondence and generation based on settings.

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

#### Usage

```js
import { PasswordInput } from "asc-web-components";

const settings = {
  minLength: 6,
  upperCase: false,
  digits: false,
  specSymbols: false
};

<PasswordInput
  inputName="demoPasswordInput"
  emailInputName="demoEmailInput"
  inputValue={value}
  onChange={e => {
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
  onValidateInput={a => console.log(a)}
  onCopyToClipboard={b => console.log("Data " + b + " copied to clipboard")}
/>;
```

#### Properties

| Props                    | Type     | Required | Values             | Default         | Description                                                           |
| ------------------------ | -------- | :------: | ------------------ | --------------- | --------------------------------------------------------------------- |
| `id`                     | `string` |    -     | -                  | -               | Allows you to set the component id                                    |
| `autoComplete`           | `string` |    -     | -                  | `new-password`  | Allows you to set the component auto-complete                         |
| `inputType`              | `array`  |    -     | `text`, `password` | `password`      | It is necessary for correct display of values ​​inside input          |
| `inputName`              | `string` |    -     | -                  | `passwordInput` | Input name                                                            |
| `emailInputName`         | `string` |    ✅    | -                  | -               | Required to associate password field with email field                 |
| `inputValue`             | `string` |    -     | -                  | -               | Input value                                                           |
| `onChange`               | `func`   |    -     | -                  | -               | Will be triggered whenever an PasswordInput typing                    |
| `clipActionResource`     | `string` |    -     | -                  | -               | Translation of text for copying email data and password               |
| `clipEmailResource`      | `string` |    -     | -                  | `E-mail`        | Text translation email to copy                                        |
| `clipPasswordResource`   | `string` |    -     | -                  | `Password`      | Text translation password to copy                                     |
| `tooltipPasswordTitle`   | `string` |    -     | -                  | -               | Text translation tooltip                                              |
| `tooltipPasswordLength`  | `string` |    -     | -                  | -               | Password text translation is long tooltip                             |
| `tooltipPasswordDigits`  | `string` |    -     | -                  | -               | Digit text translation tooltip                                        |
| `tooltipPasswordCapital` | `string` |    -     | -                  | -               | Capital text translation tooltip                                      |
| `tooltipPasswordSpecial` | `string` |    -     | -                  | -               | Special text translation tooltip                                      |
| `generatorSpecial`       | `string` |    -     | -                  | `!@#$%^&*`      | Set of special characters for password generator and validator        |
| `passwordSettings`       | `object` |    ✅    | -                  | -               | Set of settings for password generator and validator                  |
| `isDisabled`             | `bool`   |    -     | -                  | `false`         | Set input disabled                                                    |
| `inputWidth`             | `string` |    -     | -                  | -               | If you need to set input width manually                               |
| `onValidateInput`        | `func`   |    -     | -                  | -               | Will be triggered whenever an PasswordInput typing, return bool value |
| `onCopyToClipboard`      | `func`   |    -     | -                  | -               | Will be triggered if you press copy button, return formatted value    |

#### passwordSettings properties

| Props         | Type     | Required | Values | Default | Description                     |
| ------------- | -------- | :------: | ------ | ------- | ------------------------------- |
| `minLength`   | `number` |    ✅    | -      | -       | Minimum password length         |
| `upperCase`   | `bool`   |    ✅    | -      | -       | Must contain uppercase letters  |
| `digits`      | `bool`   |    ✅    | -      | -       | Must contain digits             |
| `specSymbols` | `bool`   |    ✅    | -      | -       | Must contain special characters |
