import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";

import { inject, observer } from "mobx-react";

import withLoading from "../../../../../HOCs/withLoading";
import globalColors from "@appserver/components/utils/globalColors";
import styled from "styled-components";
import TabContainer from "@appserver/components/tabs-container";
import Preview from "./settingsAppearance/preview";
const StyledComponent = styled.div`
  .container {
    display: flex;
  }

  .box {
    width: 46px;
    height: 46px;
    margin-right: 12px;
  }
  .first-color-scheme {
    background: ${globalColors.firstDefaultСolorScheme};
  }
  .second-color-scheme {
    background: ${globalColors.secondDefaultСolorScheme};
  }
  .third-color-scheme {
    background: ${globalColors.thirdDefaultСolorScheme};
  }
  .fourth-color-scheme {
    background: ${globalColors.fourthDefaultСolorScheme};
  }
  .fifth-color-scheme {
    background: ${globalColors.fifthDefaultСolorScheme};
  }
  .sixth-color-scheme {
    background: ${globalColors.sixthDefaultСolorScheme};
  }
  .sevent-color-scheme {
    background: ${globalColors.seventhDefaultСolorScheme};
  }
`;

const Appearance = (props) => {
  const array_items = [
    {
      key: "0",
      title: "Light theme",
      content: <Preview />,
    },
    {
      key: "1",
      title: "Dark theme",
      content: <div>Tab 2 content</div>,
    },
  ];

  return (
    <StyledComponent>
      <div>Color</div>
      <div>Header color is displayed only when light theme is applied</div>
      <div className="container">
        <div className="box first-color-scheme"></div>
        <div className="box second-color-scheme"></div>
        <div className="box third-color-scheme"></div>
        <div className="box fourth-color-scheme"></div>
        <div className="box fifth-color-scheme"></div>
        <div className="box sixth-color-scheme"></div>
        <div className="box sevent-color-scheme"></div>
      </div>
      <div>Preview</div>
      <TabContainer elements={array_items} />
    </StyledComponent>
  );
};

export default inject(({ auth, setup, common }) => {})(
  withLoading(withTranslation(["Settings", "Common"])(observer(Appearance)))
);
