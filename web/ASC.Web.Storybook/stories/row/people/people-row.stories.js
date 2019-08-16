import React from 'react'
import { storiesOf } from '@storybook/react'
//import withReadme from 'storybook-readme/with-readme'
//import Readme from './README.md'
import { PeopleRow } from 'asc-web-components'

storiesOf('Components|Row', module)
  //.addDecorator(withReadme(Readme))
  .add('people', () => (
    <>
      <PeopleRow
        displayName='Ashley Kinney'
        department='Direction'
        phone='+1 816 5792792'
        email='amos2006@yahoo.com'
      />
      <PeopleRow
        status='pending'
        displayName='Jesus Reddin'
        phone='+1 231 7887107'
        email='brock2008@gmail.com'
      />
      <PeopleRow
        status='disabled'
        displayName='Daniel Michael Blake Day-Lewis'
        phone='+1 804 3391052'
        email='nestor.langwor@gmail.com'
      />
      <PeopleRow
        displayName='Mel Colm-Cille Gerard Gibson'
        department='Dev, Lorem'
        phone='+1 480 6038269'
        email='jarrett.hil@hotmail.com'
      />
      <PeopleRow
        displayName='William Grant'
        department='Managment'
        phone='+1 253 8468015'
        email='austyn_dar10@gmail.com'
      />
      <PeopleRow
        displayName='Adrian A Boyd'
        phone='+1 510 3328530'
        email='annetta.runolfsdott@yahoo.com'
      />
    </>
  ));