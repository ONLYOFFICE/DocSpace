import React, { useState } from 'react';
import TableRow from '@docspace/components/table-container/TableRow';
import TableCell from '@docspace/components/table-container/TableCell';
import styled from 'styled-components';
import { FailBadge } from '../FailBadge';
import { SuccessBadge } from '../SuccessBadge';
import { ToggleButton } from '@docspace/components';
import ContextMenuButton from '@docspace/components/context-menu-button';
import DropDownItem from '@docspace/components/drop-down-item';
import SettingsIcon from 'PUBLIC_DIR/images/settings.webhooks.react.svg?url';
import HistoryIcon from 'PUBLIC_DIR/images/history.react.svg?url';
import RetryIcon from 'PUBLIC_DIR/images/refresh.react.svg?url';
import DeleteIcon from 'PUBLIC_DIR/images/delete.react.svg?url';

const Row = styled(TableRow)`
  display: flex;
  flex-direction: row;
`;

export const WebhooksListRow = ({ row, index, setRows }) => {
  const [isChecked, setIsChecked] = useState(row.isEnabled);

  const [isOpen, setIsOpen] = useState(false);

  const onClickHandler = () => {
    setIsOpen((prevIsOpen) => !prevIsOpen);
  };

  const getData = () => {
    return [
      {
        key: 'key1',
        label: (
          <DropDownItem
            isSeparator={false}
            isHeader={false}
            label="Settings"
            icon={SettingsIcon}
            onClick={() => console.log('Button 1 clicked')}
          />
        ),
        onClick: () => console.log('label1'),
      },
      {
        key: 'key2',
        label: (
          <DropDownItem
            isSeparator={false}
            isHeader={false}
            label="Webhook history"
            icon={HistoryIcon}
            onClick={() => console.log('Button 1 clicked')}
          />
        ),
        onClick: () => console.log('label2'),
      },
      {
        key: 'key3',
        label: (
          <DropDownItem
            isSeparator={false}
            isHeader={false}
            label="Retry"
            icon={RetryIcon}
            onClick={() => console.log('Button 1 clicked')}
          />
        ),
        onClick: () => console.log('label2'),
      },
      {
        key: 'key4',
        label: <DropDownItem isSeparator={true} />,
        onClick: () => console.log('label2'),
      },
      {
        key: 'key5',
        label: (
          <DropDownItem
            isSeparator={false}
            isHeader={false}
            label="Delete webhook"
            icon={DeleteIcon}
            onClick={() => console.log('Button 1 clicked')}
          />
        ),
        onClick: () => console.log('label2'),
      },
    ];
  };

  return (
    <Row>
      <TableCell>
        {row.name.text}{' '}
        {row.name.responseStatus === 'success' ? (
          <SuccessBadge label={row.name.badgeTitle} />
        ) : row.name.responseStatus === 'error' ? (
          <FailBadge label={row.name.badgeTitle} />
        ) : (
          <span></span>
        )}
      </TableCell>
      <TableCell>{row.url}</TableCell>
      <TableCell>
        <ToggleButton
          className="toggle className"
          id="toggle id"
          isChecked={isChecked}
          onChange={() => {
            setRows((prevRows) => {
              prevRows[index].isEnabled = !prevRows[index].isEnabled;
              return prevRows;
            });
            setIsChecked((prevIsChecked) => !prevIsChecked);
          }}
        />
      </TableCell>

      <TableCell>
        <div>
          <ContextMenuButton
            directionX="right"
            getData={getData}
            isDisabled={false}
            opened={isOpen}
            onClick={onClickHandler}
            title="Actions"
          />
        </div>
      </TableCell>
    </Row>
  );
};
