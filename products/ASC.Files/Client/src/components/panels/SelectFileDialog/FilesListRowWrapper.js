import React from "react";
import { Provider as MobxProvider } from "mobx-react";
import FilesListRow from "./FilesListRow";
import stores from "../../../store/index";

const ListRowWrapper = (props) => <FilesListRow {...props} />;

class ListRow extends React.Component {
  render() {
    return (
      <MobxProvider {...stores}>
        <ListRowWrapper {...this.props} />
      </MobxProvider>
    );
  }
}

export default ListRow;
