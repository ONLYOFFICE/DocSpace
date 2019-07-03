import React from 'react';
import { storiesOf } from '@storybook/react';
import { Layout } from 'asc-web-components'

storiesOf('Components|Layout', module)
  .add('Layout', () => (
    <Layout>
      <div style={{padding: 40}}>
          Page Content
      </div>
    </Layout>
  ));
