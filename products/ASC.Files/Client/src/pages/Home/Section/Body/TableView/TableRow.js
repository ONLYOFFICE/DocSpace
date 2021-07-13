import React from "react";
import { withRouter } from "react-router";
import withContent from "../../../../../HOCs/withContent";
import withBadges from "../../../../../HOCs/withBadges";
import withFileActions from "../../../../../HOCs/withFileActions";
import withContextOptions from "../../../../../HOCs/withContextOptions";
import { withTranslation } from "react-i18next";
import TableRow from "@appserver/components/table-container/TableRow";
import TableCell from "@appserver/components/table-container/TableCell";
import CheckboxCell from "./sub-components/CheckboxCell";
import FileNameCell from "./sub-components/FileNameCell";
import SizeCell from "./sub-components/SizeCell";
import AuthorCell from "./sub-components/AuthorCell";
import CreatedCell from "./sub-components/CreatedCell";
import globalColors from "@appserver/components/utils/globalColors";

const sideColor = globalColors.gray;

const FilesTableRow = (props) => {
  return (
    <TableRow>
      <CheckboxCell {...props} />

      <TableCell>
        <FileNameCell {...props} />
      </TableCell>
      <TableCell>
        <AuthorCell sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell>
        <CreatedCell sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell>
        <SizeCell sideColor={sideColor} {...props} />
      </TableCell>
      <TableCell></TableCell>
    </TableRow>
  );
};

export default withTranslation("Home")(
  withFileActions(
    withRouter(withContextOptions(withContent(withBadges(FilesTableRow))))
  )
);
