# Toast

Toast allow you to add notification to your page with ease.

`<Toast />` is container for your notification. Remember to render the `<Toast />` _once_ in your application tree. If you can't figure out where to put it, rendering it in the application root would be the best bet.

`toastr` is a function for showing notifications.

### Usage

```js
import Toast from "@docspace/components/toast";
import toastr from "@docspace/components/toast/toastr";
```

```jsx
<Toast />
<button onClick={() => toastr.success('Some text for toast', 'Some text for title', true)}>Click</button>
```

or

```jsx
<Toast>{toastr.success("Some text for toast")}</Toast>
```

You can use simple html tags. For this action you should wrap your message by empty tags:

```jsx
<Toast />
<button onClick={() => toastr.success(<>You have <b>bold text</b></>)}>Click</button>
```

If your notification include only text in html tags or data in JSX tags, you can omit empty tags:

```jsx
<Toast />
<button onClick={() => toastr.success(<b>Bold text</b>)}>Click</button>
```

#### Other Options

```js
<Toast/>
    // Remove all toasts in your page programmatically
<button onClick = {()=> { toastr.clear() }}>Clear</button>
```

### Properties

| Props       |        Type        | Required |                Values                 | Default | Description                                                                                                                    |
| ----------- | :----------------: | :------: | :-----------------------------------: | :-----: | ------------------------------------------------------------------------------------------------------------------------------ |
| `className` |      `string`      |    -     |                   -                   |    -    | Accepts class                                                                                                                  |
| `data`      | `element`,`string` |    -     |                   -                   |    -    | Any components or data inside a toast                                                                                          |
| `id`        |      `string`      |    -     |                   -                   |    -    | Accepts id                                                                                                                     |
| `style`     |   `obj`, `array`   |    -     |                   -                   |    -    | Accepts css style                                                                                                              |
| `timeout`   |      `number`      |    -     |     `from 750ms`, `0 - disabling`     | `5000`  | Time (in milliseconds) for showing your toast. Setting in `0` let you to show toast constantly until clicking on it            |
| `title`     |      `string`      |    -     |                   -                   |    -    | Title inside a toast                                                                                                           |
| `type`      |      `oneOf`       |    ✅    | `success`, `error`, `warning`, `info` |    -    | Define color and icon of toast                                                                                                 |
| `withCross` |       `bool`       |    -     |                   -                   | `false` | If `false`: toast disappeared after clicking on any area of toast. If `true`: toast disappeared after clicking on close button |
