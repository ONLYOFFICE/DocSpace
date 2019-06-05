import React from 'react';
import 'reactstrap';
import { storiesOf } from '@storybook/react';
import { LoginForm } from 'asc-web-components';

function onSubmit(e, credentials) {
    console.log("onSubmit", e, credentials);
}

storiesOf('Components|Examples. Forms', module)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: 'responsive' } })
  .add('login', () => (
      <LoginForm loginPlaceholder='You registration email' passwordPlaceholder='Password' buttonText='Sign In' onSubmit={onSubmit} />
  ));