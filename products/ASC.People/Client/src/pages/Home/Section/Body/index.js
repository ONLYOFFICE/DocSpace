import React from "react";
import { inject, observer } from "mobx-react";
import Loaders from "@appserver/common/components/Loaders";
import { withTranslation } from "react-i18next";
import withLoader from "../../../../HOCs/withLoader";
import PeopleRowContainer from "./RowView/PeopleRowContainer";
import TableView from "./TableView/TableContainer";
import { Consumer } from "@appserver/components/utils/context";
import { CatalogMainButtonContent } from "../../../../components/Catalog";
import { isMobile } from "react-device-detect";
import {
  isMobile as isMobileUtils,
  isTablet as isTabletUtils,
} from "@appserver/components/utils/device";

class SectionBodyContent extends React.Component {
  render() {
    const { tReady, viewAs } = this.props;

    return (
      <Consumer>
        {(context) =>
          viewAs === "table" ? (
            <>
              <TableView sectionWidth={context.sectionWidth} tReady={tReady} />
              {(isMobile || isMobileUtils() || isTabletUtils()) && (
                <CatalogMainButtonContent sectionWidth={context.sectionWidth} />
              )}
            </>
          ) : (
            <>
              <PeopleRowContainer
                sectionWidth={context.sectionWidth}
                tReady={tReady}
              />
              {(isMobile || isMobileUtils() || isTabletUtils()) && (
                <CatalogMainButtonContent sectionWidth={context.sectionWidth} />
              )}
            </>
          )
        }
      </Consumer>
    );
  }
}

export default inject(({ peopleStore }) => {
  const { viewAs } = peopleStore;

  return { viewAs };
})(
  withTranslation(["Home", "Common", "Translations"])(
    withLoader(observer(SectionBodyContent))(
      <Loaders.Rows isRectangle={false} />
    )
  )
);
