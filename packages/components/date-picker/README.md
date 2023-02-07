# DatePicker

Base DatePicker component

### Usage

```js
import DatePicker from "@docspace/components/date-picker";
```

```jsx
<DatePicker
  onChange={(date) => {
    console.log("Selected date", date);
  }}
  selectedDate={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date(new Date().getFullYear() + 1 + "/01/01")}
  isDisabled={false}
  isReadOnly={false}
  hasError={false}
  isOpen={false}
  themeColor="#ED7309"
  locale="en"
/>
```

#### Properties

| Props                   | Type           | Required | Values                      | Default                   | Description                                        |
| ----------------------- | -------------- | :------: | --------------------------- | ------------------------- | -------------------------------------------------- |
| `calendarHeaderContent` | `string`       |    -     | -                           | -                         | Calendar header content (calendar opened in aside) |
| `calendarSize`          | `oneOf`        |    -     | `base`, `big`               | -                         | Calendar size                                      |
| `className`             | `string`       |    -     | -                           | -                         | Accepts class                                      |
| `displayType`           | `oneOf`        |    -     | `dropdown`, `aside`, `auto` | `auto`                    | Calendar display type                              |
| `hasError`              | `bool`         |    -     |                             | -                         | Set error date-input style                         |
| `id`                    | `string`       |    -     | -                           | -                         | Accepts id                                         |
| `isDisabled`            | `bool`         |    -     | -                           | -                         | Disabled react-calendar                            |
| `isOpen`                | `bool`         |    -     |                             | -                         | Opens calendar                                     |
| `isReadOnly`            | `bool`         |    -     |                             | -                         | Set input type is read only                        |
| `locale`                | `string`       |    -     | -                           | `User's browser settings` | Browser locale                                     |
| `maxDate`               | `date`         |    -     | -                           | `new Date("3000/01/01")`  | Maximum date that the user can select.             |
| `minDate`               | `date`         |    -     | -                           | `new Date("1970/01/01")`  | Minimum date that the user can select.             |
| `onChange`              | `func`         |    -     | -                           | -                         | Function called when the user select a day         |
| `openToDate`            | `date`         |    -     | -                           | `(today)`                 | Opened date value                                  |
| `scaled`                | `bool`         |    -     |                             | -                         | Selected calendar size                             |
| `selectedDate`          | `date`         |    -     | -                           | `(today)`                 | Selected date value                                |
| `style`                 | `obj`, `array` |    -     | -                           | -                         | Accepts css style                                  |
| `themeColor`            | `string`       |    -     | -                           | `#ED7309`                 | Color of the selected day                          |
| `zIndex`                | `number`       |    -     | -                           | `310`                     | Calendar css z-index                               |
