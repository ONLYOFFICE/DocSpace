import React from 'react';
import { storiesOf } from '@storybook/react';
import withReadme from 'storybook-readme/with-readme'
import Readme from './README.md'
import RowContent from '.';
import Link from '../link';
import { Icons } from '../icons';

storiesOf('Components|RowContent', module)
  .addDecorator(withReadme(Readme))
  .add('base', () => {

    return (
      <>
        <RowContent>
          <Link type='page' title='Demo' isBold={true} fontSize={15} color='#333333' >Demo</Link>
          <>
            <Icons.SendClockIcon size='small' isfill={true} color='#3B72A7' />
            <Icons.CatalogSpamIcon size='small' isfill={true} color='#3B72A7' />
          </>
          <Link type='page' title='Demo' fontSize={12} color='#A3A9AE' >Demo</Link>
          <Link type='action' title='Demo' fontSize={12} color='#A3A9AE' >Demo</Link>
          <Link type='page' title='0 000 0000000' fontSize={12} color='#A3A9AE' >0 000 0000000</Link>
          <Link type='page' title='demo@demo.com' fontSize={12} color='#A3A9AE' >demo@demo.com</Link>
        </RowContent>
        <RowContent>
          <Link type='page' title='Demo Demo' isBold={true} fontSize={15} color='#333333' >Demo Demo</Link>
          <>
            <Icons.CatalogSpamIcon size='small' isfill={true} color='#3B72A7' />
          </>
          <></>
          <Link type='action' title='Demo Demo' fontSize={12} color='#A3A9AE' >Demo Demo</Link>
          <Link type='page' title='0 000 0000000' fontSize={12} color='#A3A9AE' >0 000 0000000</Link>
          <Link type='page' title='demo.demo@demo.com' fontSize={12} color='#A3A9AE' >demo.demo@demo.com</Link>
        </RowContent>
        <RowContent>
          <Link type='page' title='Demo Demo Demo' isBold={true} fontSize={15} color='#333333' >Demo Demo Demo</Link>
          <></>
          <></>
          <Link type='action' title='Demo Demo Demo' fontSize={12} color='#A3A9AE' >Demo Demo Demo</Link>
          <Link type='page' title='0 000 0000000' fontSize={12} color='#A3A9AE' >0 000 0000000</Link>
          <Link type='page' title='demo.demo.demo@demo.com' fontSize={12} color='#A3A9AE' >demo.demo.demo@demo.com</Link>
        </RowContent>
        <RowContent>
          <Link type='page' title='Demo Demo Demo Demo' isBold={true} fontSize={15} color='#333333' >Demo Demo Demo Demo</Link>
          <>
            <Icons.SendClockIcon size='small' isfill={true} color='#3B72A7' />
          </>
          <Link type='page' title='Demo' fontSize={12} color='#A3A9AE' >Demo</Link>
          <Link type='action' title='Demo Demo Demo Demo' fontSize={12} color='#A3A9AE' >Demo Demo Demo Demo</Link>
          <Link type='page' title='0 000 0000000' fontSize={12} color='#A3A9AE' >0 000 0000000</Link>
          <Link type='page' title='demo.demo.demo.demo@demo.com' fontSize={12} color='#A3A9AE' >demo.demo.demo.demo@demo.com</Link>
        </RowContent>
      </>
    );
  });