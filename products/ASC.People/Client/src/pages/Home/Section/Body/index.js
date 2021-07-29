import React from "react";
import { inject, observer } from "mobx-react";
import Loaders from "@appserver/common/components/Loaders";
import { isTabletView } from "@appserver/components/utils/device";
import { withTranslation } from "react-i18next";
import withLoader from "../../../../HOCs/withLoader";
import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";

class SectionBodyContent extends React.Component {
  onResize = () => {
    const tabletView = isTabletView();
    const { viewAs, setViewAs } = this.props;

    if (tabletView) {
      viewAs !== "table" && setViewAs("table");
    } else {
      viewAs !== "row" && setViewAs("row");
    }
  };

  componentDidMount() {
    this.onResize();
    window.addEventListener("resize", this.onResize);
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.onResize);
  }

  render() {
    const { tReady, viewAs } = this.props;

    return viewAs === "table" ? (
      <TableView tReady={tReady} />
    ) : (
      <PeopleRowContainer tReady={tReady} />
    );
  }
}

export default inject(({ peopleStore }) => {
  const { viewAs, setViewAs } = peopleStore;

  return { viewAs, setViewAs };
})(
  withTranslation("Home")(
    withLoader(observer(SectionBodyContent))(
      <Loaders.Rows isRectangle={false} />
    )
  )
);
