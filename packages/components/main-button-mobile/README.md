# MainButtonMobile

### Usage

```js
import MainButtonMobile from "@docspace/components/main-button-mobile";
```

```jsx
const actionOptions = [
  {
    key: "1",
    label: "New document",
    icon: "static/images/mobile.actions.document.react.svg",
  },
  {
    key: "2",
    label: "New presentation",
    icon: "static/images/mobile.actions.presentation.react.svg",
  },
];

const buttonOptions = [
  {
    key: "1",
    label: "Import point",
  },
  {
    key: "2",
    label: "Import point",
  },
];

const progressOptions = [
  {
    key: "1",
    label: "Uploads",
    percent: 30,
    status: `8/10`,
    open: true,
  },
];

<MainButtonMobile
  style={{
    top: "90%",
    left: "82%",
    position: "fixed",
  }}
  manualWidth="320px"
  title="Upload"
  withButton={true}
  actionOptions={actionOptions}
  progressOptions={progressOptions}
  isOpenButton={true}
  buttonOptions={buttonOptions}
/>;
```

| Props             |      Type      | Required | Values | Default | Description                                                                                        |
| ----------------- | :------------: | :------: | :----: | :-----: | -------------------------------------------------------------------------------------------------- |
| `style`           | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                                                  |
| `actionOptions`   |     `obj`      |    -     |   -    |    -    | Options for drop down items                                                                        |
| `progressOptions` |     `obj`      |    -     |   -    |    -    | If you need display progress bar components                                                        |
| `buttonOptions`   |     `obj`      |    -     |   -    |    -    | Menu that opens by clicking on the button                                                          |
| `onUploadClick`   |     `func`     |    -     |   -    |    -    | The function that will be called after the button click                                            |
| `withButton`      |     `bool`     |    -     |   -    |    -    | Show button inside drop down                                                                       |
| `isOpenButton`    |     `bool`     |    -     |   -    |    -    | The parameter that is used with buttonOptions is needed to open the menu by clicking on the button |
| `title`           |    `string`    |    -     |        |    -    | The name of the button in the drop down                                                            |
| `percent`         |    `number`    |    -     |   -    |    -    | Loading indicator                                                                                  |
| `manualWidth`     |    `string`    |    -     |   -    |    -    | Required if you need to specify the exact width of the drop down component                         |
| `opened`          |     `bool`     |    -     |   -    |    -    | Tells when the dropdown should be opened                                                           |
| `className`       |    `string`    |    -     |   -    |    -    | Accepts class                                                                                      |
| `onClose`         |     `func`     |    -     |   -    |    -    | if you need close drop down                                                                        |
