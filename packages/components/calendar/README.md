# Calendar

Used to display custom calendar

### Usage

```js
import Calendar from "@docspace/components/calendar";
```

```jsx
<Calendar
  onChange={(date) => {
    console.log("Selected date:", date);
  }}
  disabled={false}
  themeColor="#ED7309"
  selectedDate={new Date()}
  openToDate={new Date()}
  minDate={new Date("1970/01/01")}
  maxDate={new Date("3000/01/01")}
  locale="ru"
/>
```

#### Properties

| Props          |      Type      | Required |    Values     |          Default          | Description                                                  |
| -------------- | :------------: | :------: | :-----------: | :-----------------------: | ------------------------------------------------------------ |
| `className`    |    `string`    |    -     |       -       |             -             | Accepts class                                                |
| `id`           |    `string`    |    -     |       -       |             -             | Accepts id                                                   |
| `isDisabled`   |     `bool`     |    -     |       -       |             -             | Disabled react-calendar                                      |
| `locale`       |    `string`    |    -     |       -       | `User's browser settings` | Browser locale                                               |
| `maxDate`      |     `date`     |    -     |       -       | `new Date("3000/01/01")`  | Maximum date that the user can select.                       |
| `minDate`      |     `date`     |    -     |       -       | `new Date("1970/01/01")`  | Minimum date that the user can select.                       |
| `onChange`     |     `func`     |    -     |       -       |             -             | Function called when the user select a day                   |
| `openToDate`   |     `date`     |    -     |       -       |       `new Date()`        | The beginning of a period that shall be displayed by default |
| `selectedDate` |     `date`     |    -     |       -       |       `new Date()`        | Selected date value                                          |
| `size`         |    `oneOf`     |    -     | `base`, `big` |          `base`           | Calendar size                                                |
| `style`        | `obj`, `array` |    -     |       -       |             -             | Accepts css style                                            |
| `themeColor`   |    `string`    |    -     |       -       |         `#ED7309`         | Color of the selected day                                    |
