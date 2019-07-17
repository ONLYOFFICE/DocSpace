# Toast

## Usage

```js
import { Toast, toastr } from 'asc-web-components';
```

#### Description

Toast allow you to add notification to your page with ease.
`<Toast />` is container for your notification. Remember to render the `<Toast />` *once* in your application tree. If you can't figure out where to put it, rendering it in the application root would be the best bet.
`toastr` is a function for showing notifications.

#### Usage

```js
<Toast />
<button onClick={() => toastr.success('Some text for toast', 'Some text for title', true)}>Click</button>
```

or 

```js
<Toast>
  {toastr.success('Some text for toast')}
</Toast>
```


#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `type`             | `oneOf`  |    ✅    | success, error, warning, info | -     | Define color and icon of toast                                           |
| `text`             | `string` |    -     | -                             | -     | Text inside a toast                                                      |
| `title`            | `string` |    -     | -                             | -     | Title inside a toast                                                     |
| `autoClosed`       | `bool`   |    -     | true, false                   | `true`|If `true`: toast disappeared after 5 seconds or after clicking on any area of toast. If `false`: included close button, toast can be closed by clicking on it.|

#### Other Options
```js
<Toast/>
    // Remove all toasts in your page programmatically
    <button onClick = {()=> { toastr.clear() }}>Clear</button>
```

#### Examples
```js
<Toast>
    // Display a warning toast, with no title
    {toastr.warning('My name is Dan. I like my cat')}

    // Display a success toast, with a title
    {toastr.success('Have fun storming the castle!', 'Miracle Max Says')}

    // Display a error toast, with title and you should close it manually
    {toastr.error('I do not think that word means what you think it means.', 'Inconceivable!', false)}
</Toast>
```
