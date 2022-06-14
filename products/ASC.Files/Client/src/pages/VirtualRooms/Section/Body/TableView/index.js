import React from "react";
import { inject, observer } from "mobx-react";
import elementResizeDetectorMaker from "element-resize-detector";

import { isMobile } from "react-device-detect";

import TableContainer from "@appserver/components/table-container";
import TableBody from "@appserver/components/table-container/TableBody";

import TableHeaderContent from "./sub-components/TableHeader";
import Row from "./sub-components/TableRow";

const TABLE_VERSION = "1";
const TABLE_COLUMNS = `roomsTableColumns_ver-${TABLE_VERSION}`;
const COLUMNS_SIZE = `roomsColumnsSize_ver-${TABLE_VERSION}`;

const tagMaxWidth = 100;
const gridGap = 4;

const VirtualRoomsTable = ({
  viewAs,
  setViewAs,
  userId,
  data,
  sectionWidth,
  theme,
  getRoomsContextOptions,
}) => {
  const [tagCount, setTagCount] = React.useState(null);

  const containerRef = React.useRef(null);
  const firstRowRef = React.useRef(null);

  React.useEffect(() => {
    if ((viewAs !== "table" && viewAs !== "row") || !setViewAs) return;

    if (sectionWidth < 1025 || isMobile) {
      viewAs !== "row" && setViewAs("row");
    } else {
      viewAs !== "table" && setViewAs("table");
    }
  }, [sectionWidth]);

  React.useEffect(() => {
    if (!firstRowRef?.current) return;

    onResize();

    const elementResizeDetector = elementResizeDetectorMaker({
      strategy: "scroll",
      callOnAdd: false,
    });

    elementResizeDetector.listenTo(firstRowRef.current, onResize);

    return () => {
      if (!firstRowRef?.current) return;

      elementResizeDetector.uninstall(firstRowRef.current);
    };
  }, [firstRowRef, data]);

  const onResize = React.useCallback(() => {
    if (firstRowRef?.current) {
      const { width } = firstRowRef.current.getBoundingClientRect();

      const widthWithoutPadding = width;

      const columns = Math.floor(widthWithoutPadding / (tagMaxWidth + gridGap));

      if (columns != tagCount) setTagCount(columns);
    }
  }, [firstRowRef, tagCount]);

  const tableColumns = `${TABLE_COLUMNS}=${userId}`;
  const columnStorageName = `${COLUMNS_SIZE}=${userId}`;

  return (
    <TableContainer forwardedRef={containerRef}>
      <TableHeaderContent
        sectionWidth={sectionWidth}
        containerRef={containerRef}
        tableStorageName={tableColumns}
        columnStorageName={columnStorageName}
      />
      <TableBody>
        {data.map((item, index) =>
          index === 0 ? (
            <Row
              key={item.key}
              item={item}
              theme={theme}
              ref={firstRowRef}
              tagCount={tagCount}
              getContextModel={getRoomsContextOptions}
            />
          ) : (
            <Row
              key={item.key}
              item={item}
              theme={theme}
              tagCount={tagCount}
              getContextModel={getRoomsContextOptions}
            />
          )
        )}
      </TableBody>
    </TableContainer>
  );
};

export default inject(({ auth, filesStore, contextOptionsStore }) => {
  const { settingsStore } = auth;

  const { theme } = settingsStore;

  const { viewAs, setViewAs } = filesStore;

  const { getRoomsContextOptions } = contextOptionsStore;

  return {
    theme,
    viewAs,
    setViewAs,
    getRoomsContextOptions,
    userId: auth.userStore.user.id,
  };
})(observer(VirtualRoomsTable));
