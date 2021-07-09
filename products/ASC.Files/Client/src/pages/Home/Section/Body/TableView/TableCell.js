import React from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import { withTranslation } from "react-i18next";
import TableCell from "@appserver/components/table-container/TableCell";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";

const TableCellFiles = (props) => {
  const { column } = props;

  const getElem = () => {
    let field = null;
    for (let inc of column.includes) {
      if (field) {
        field = field[inc];
      } else {
        field = inc;
      }
    }

    switch (field) {
      case "title":
        return <FileNameCell {...props} />;
      case "contentLength":
        return <SizeCell {...props} />;

      default:
        return <></>;
    }
  };

  return <TableCell {...props}>{getElem()}</TableCell>;
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(TableCellFiles))))
  )
);
