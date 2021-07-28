import React from "react";
import { withTranslation } from "react-i18next";
import Dialogs from "./Dialogs";
import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@appserver/common/components/Loaders";

import PeopleRowContainer from "./RowView/PeopleRowContainer";
//import TableView from "./TableView/TableContainer";

const SectionBodyContent = ({ tReady }) => {
  return (
    <>
      <PeopleRowContainer tReady={tReady} />
      {/* <TableView tReady={tReady} /> */}
      <Dialogs />
    </>
  );
};

export default withTranslation("Home")(
  withLoader(SectionBodyContent)(<Loaders.Rows isRectangle={false} />)
);
