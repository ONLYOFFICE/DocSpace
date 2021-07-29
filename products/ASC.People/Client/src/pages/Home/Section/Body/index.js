import React from "react";
import { withTranslation } from "react-i18next";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";

import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";

const SectionBodyContent = ({ tReady }) => {
  return (
    <>
      <PeopleRowContainer tReady={tReady} />
      {/* <TableView tReady={tReady} /> */}
    </>
  );
};

export default withTranslation("Home")(
  withLoader(SectionBodyContent)(<Loaders.Rows isRectangle={false} />)
);
