import React, { useState, useRef } from 'react';

import styled from 'styled-components';

import TableContainer from '@docspace/components/table-container/TableContainer';
import TableHeader from '@docspace/components/table-container/TableHeader';
import TableBody from '@docspace/components/table-container/TableBody';

import { WebhooksListRow } from '../WebhooksListRow';

const Header = styled(TableHeader)`
  display: flex;
`;

const TableWrapper = styled(TableContainer)`
  margin-top: 0;
`;

export const WebhooksList = () => {
  const wrapperRef = useRef(null);
  const headerRef = useRef(null);

  const [columns, setColumns] = useState([
    {
      key: 'Name',
      title: 'Name',
      enable: true,
      default: true,
      active: true,
      minWidth: 200,
    },
    {
      key: 'URL',
      title: 'URL',
      enable: true,
      resizable: true,
    },
    {
      key: 'State',
      title: 'State',
      enable: true,
      resizable: true,
    },
  ]);

  const [rows, setRows] = useState([
    {
      name: {
        text: 'Webhook 1',
        badgeTitle: '200',
        responseStatus: 'success',
      },
      url: 'https://webhook.site/3d9f41d8-30dc-4f55-8b78-1649f4118c56',
      isEnabled: true,
    },
    {
      name: {
        text: 'Webhook 2',
        badgeTitle: '404',
        responseStatus: 'error',
      },
      url: 'https://webhook.site/3d9f41d8-30dc-4f55-8b78-16',
      isEnabled: false,
    },
  ]);

  return (
    <TableWrapper forwardedRef={wrapperRef}>
      <Header
        columns={columns}
        containerRef={headerRef}
        sectionWidth={1000}
        checkboxMargin="12px"
      />
      <TableBody>
        {rows.map((row, index) => (
          <WebhooksListRow key={row.url} row={row} index={index} setRows={setRows} />
        ))}
      </TableBody>
    </TableWrapper>
  );
};

// containerRef: PropTypes.shape({ current: PropTypes.any }).isRequired,
// columns: PropTypes.array.isRequired,
// sortBy: PropTypes.string,
// sorted: PropTypes.bool,
// columnStorageName: PropTypes.string,
// sectionWidth: PropTypes.number,
// onClick: PropTypes.func,
// resetColumnsSize: PropTypes.bool,
// isLengthenHeader: PropTypes.bool,
// sortingVisible: PropTypes.bool,
// infoPanelVisible: PropTypes.bool,
// useReactWindow: PropTypes.bool,
// showSettings: PropTypes.bool,
