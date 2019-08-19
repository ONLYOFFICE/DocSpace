import React from 'react';
import { storiesOf } from '@storybook/react';
//import withReadme from 'storybook-readme/with-readme'
//import Readme from './README.md'
import { Row } from 'asc-web-components';

storiesOf('Components|Row', module)
  //.addDecorator(withReadme(Readme))
  .add('people', () => (
    <>
      <Row.People
        displayName='Ashley Kinney'
        department='Direction'
        phone='+1 816 5792792'
        email='amos2006@yahoo.com'
      />
      <Row.People
        status='pending'
        displayName='Jesus Reddin'
        phone='+1 231 7887107'
        email='brock2008@gmail.com'
      />
      <Row.People
        status='disabled'
        displayName='Daniel Michael Blake Day-Lewis'
        phone='+1 804 3391052'
        email='nestor.langwor@gmail.com'
      />
      <Row.People
        displayName='Mel Colm-Cille Gerard Gibson'
        department='Dev, Lorem'
        phone='+1 480 6038269'
        email='jarrett.hil@hotmail.com'
      />
      <Row.People
        displayName='William Grant'
        department='Managment'
        phone='+1 253 8468015'
        email='austyn_dar10@gmail.com'
      />
      <Row.People
        displayName='Adrian A Boyd'
        phone='+1 510 3328530'
        email='annetta.runolfsdott@yahoo.com'
      />
    </>
  ));