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
import AuthorCell from "./sub-components/AuthorCell";
import CreatedCell from "./sub-components/CreatedCell";
import globalColors from "@appserver/components/utils/globalColors";

const sideColor = globalColors.gray;

const TableCellFiles = (props) => {
  const { column, item } = props;

  const getElem = () => {
    let field = null;
    for (let inc of column.includes) {
      field = inc;
      // if (field) {
      //   console.log("123", field);
      //   console.log("inc", inc);
      //   field = field[inc];
      //   console.log("234", field);
      // } else {
      //   field = inc;
      // }
    }

    switch (field) {
      case "title":
        return <FileNameCell {...props} />;
      case "contentLength":
        return <SizeCell sideColor={sideColor} {...props} />;
      case "displayName":
        return <AuthorCell sideColor={sideColor} {...props} />;
      case "created":
        return <CreatedCell sideColor={sideColor} {...props} />;

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
