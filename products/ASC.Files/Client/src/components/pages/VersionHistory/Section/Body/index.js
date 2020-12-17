import React from "react";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import { RowContainer } from "asc-web-components";
import VersionRow from "./VersionRow";

const SectionBodyContent = (props) => {
  const { versions, culture, getFileVersions } = props;
  console.log("VersionHistory SectionBodyContent render()", versions);

  let itemVersion = null;

  return (
    <RowContainer useReactWindow={false}>
      {versions.map((info, index) => {
        let isVersion = true;
        if (itemVersion === info.versionGroup) {
          isVersion = false;
        } else {
          itemVersion = info.versionGroup;
        }

        return (
          <VersionRow
            getFileVersions={getFileVersions}
            isVersion={isVersion}
            key={info.id}
            info={info}
            index={index}
            culture={culture}
          />
        );
      })}
    </RowContainer>
  );
};

export default withRouter(withTranslation()(SectionBodyContent));
