import React, { useEffect, useRef } from 'react';
import TableContainer from '@appserver/components/table-container';
import { inject, observer } from 'mobx-react';
import TableRow from './TableRow';
import TableHeader from './TableHeader';
import TableBody from '@appserver/components/table-container/TableBody';
import { isMobile } from 'react-device-detect';

const Table = ({ filesList, sectionWidth, viewAs, setViewAs, theme }) => {
  const ref = useRef(null);

  useEffect(() => {
    if ((viewAs !== 'table' && viewAs !== 'row') || !setViewAs) return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== 'row' && setViewAs('row');
    } else {
      viewAs !== 'table' && setViewAs('table');
    }
  }, [sectionWidth]);

  return (
    <TableContainer forwardedRef={ref}>
      <TableHeader sectionWidth={sectionWidth} containerRef={ref} />
      <TableBody>
        {filesList.map((item) => (
          <TableRow key={item.id} item={item} theme={theme} />
        ))}
      </TableBody>
    </TableContainer>
  );
};

export default inject(({ filesStore, auth }) => {
  const { filesList, viewAs, setViewAs } = filesStore;

  return {
    filesList,
    viewAs,
    setViewAs,
    theme: auth.settingsStore.theme,
  };
})(observer(Table));
