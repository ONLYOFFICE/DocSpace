import React, { useRef } from "react";

import styled from "styled-components";

import TableContainer from "@docspace/components/table-container/TableContainer";
import TableBody from "@docspace/components/table-container/TableBody";
import HistoryTableHeader from "./HistoryTableHeader";
import { HistoryTableRow } from "./HistoryTableRow";

import { Consumer } from "@docspace/components/utils/context";

import { inject, observer } from "mobx-react";

const TableWrapper = styled(TableContainer)`
  margin-top: 16px;

  .table-container_row-checkbox {
    padding-left: 20px;
    width: 16px;
  }
`;

const WebhookHistoryTable = (props) => {
  const { viewAs, withPaging } = props;
  const tableRef = useRef(null);

  return (
    <Consumer>
      {(context) =>
        viewAs === "table" ? (
          <TableWrapper
            forwardedRef={tableRef}
            // useReactWindow={!withPaging}
            style={{
              gridTemplateColumns: "300px 100px 400px 24px",
            }}>
            <HistoryTableHeader sectionWidth={context.sectionWidth} tableRef={tableRef} />
            <TableBody itemHeight={49}>
              <HistoryTableRow
                key="6F9619FF-8B86-D011-B42D-00CF4FC96r32"
                eventData={{
                  id: "6F9619FF-8B86-D011-B42D-00CF4FC964DF",
                  status: 200,
                  delivery: "Mar 17, 2021, 11:10:00 PM UTC",
                }}
              />
              <HistoryTableRow
                key="6F9619FF-8B86-D011-B42D-00CF4FC96r311"
                eventData={{
                  id: "6F9619FF-8B86-D011-B42D-00CF4FC964TY",
                  status: 400,
                  delivery: "Mar 17, 2021, 11:10:00 PM UTC",
                }}
              />
              <HistoryTableRow
                key="6F9619FF-8B86-D011-B42D-00CF4FC96r3233"
                eventData={{
                  id: "6F9619FF-8B86-D011-B42D-00CF4FC96r32",
                  status: 300,
                  delivery: "Mar 17, 2021, 11:10:00 PM UTC",
                }}
              />
            </TableBody>
          </TableWrapper>
        ) : (
          <></>
        )
      }
    </Consumer>
  );
};

export default inject(({ filesStore, settingsStore }) => {
  const { viewAs } = filesStore;

  const { withPaging, theme } = settingsStore;
  return {
    viewAs,
    withPaging,
    theme,
  };
})(observer(WebhookHistoryTable));
