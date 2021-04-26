import React from "react";
import { inject, observer } from "mobx-react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { withTranslation } from "react-i18next";
import Headline from "@appserver/common/components/Headline";
import IconButton from "@appserver/components/icon-button";
import { tablet } from "@appserver/components/utils/device";
import PeopleSelector from "people/PeopleSelector";

import {
  getKeyByLink,
  settingsTree,
  getTKeyByKey,
  checkPropertyByLink,
} from "../../../utils";
import { combineUrl } from "@appserver/common/utils";
import { AppServerConfig } from "@appserver/common/constants";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  max-width: calc(100vw - 32px);

  .action-wrapper {
    flex-grow: 1;

    .action-button {
      margin-left: auto;
    }
  }

  .arrow-button {
    margin-right: 16px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);

    const { match, location } = props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;

    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/");

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    const isCategory = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isCategory"
    );
    const isHeader = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isHeader"
    );
    this.state = {
      header,
      isCategoryOrHeader: isCategory || isHeader,
      showSelector: false,
    };
  }

  componentDidUpdate() {
    const arrayOfParams = this.getArrayOfParams();

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    const isCategory = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isCategory"
    );
    const isHeader = checkPropertyByLink(
      arrayOfParams,
      settingsTree,
      "isHeader"
    );
    const isCategoryOrHeader = isCategory || isHeader;

    header !== this.state.header && this.setState({ header });
    isCategoryOrHeader !== this.state.isCategoryOrHeader &&
      this.setState({ isCategoryOrHeader });
  }

  onBackToParent = () => {
    let newArrayOfParams = this.getArrayOfParams();
    newArrayOfParams.splice(-1, 1);
    const newPath = "/settings/" + newArrayOfParams.join("/");
    this.props.history.push(combineUrl(AppServerConfig.proxyURL, newPath));
  };

  getArrayOfParams = () => {
    const { match, location } = this.props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;

    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split("/");
    return arrayOfParams;
  };

  addUsers = (items) => {
    const { addUsers } = this.props;
    if (!addUsers) return;
    addUsers(items);
  };

  onToggleSelector = (status = !this.state.showSelector) => {
    this.setState({
      showSelector: status,
    });
  };

  onCancelSelector = () => {
    this.onToggleSelector(false);
  };

  onSelect = (items) => {
    this.onToggleSelector(false);
    this.addUsers(items);
    //this.changeOwner(items[0]);
  };

  render() {
    const { t, addUsers, groupsCaption } = this.props;
    const { header, isCategoryOrHeader, showSelector } = this.state;
    const arrayOfParams = this.getArrayOfParams();

    return (
      <HeaderContainer>
        {!isCategoryOrHeader && arrayOfParams[0] && (
          <IconButton
            iconName="/static/images/arrow.path.react.svg"
            size="17"
            color="#A3A9AE"
            hoverColor="#657077"
            isFill={true}
            onClick={this.onBackToParent}
            className="arrow-button"
          />
        )}
        <Headline type="content" truncate={true}>
          {t(header)}
        </Headline>
        {addUsers && (
          <div className="action-wrapper">
            <IconButton
              iconName="/static/images/actions.header.touch.react.svg"
              size="17"
              color="#A3A9AE"
              hoverColor="#657077"
              isFill={true}
              onClick={this.onToggleSelector}
              className="action-button"
            />
            <PeopleSelector
              isMultiSelect={true}
              displayType="aside"
              isOpen={showSelector}
              onSelect={this.onSelect}
              groupsCaption={groupsCaption}
              onCancel={this.onCancelSelector}
            />
          </div>
        )}
      </HeaderContainer>
    );
  }
}

export default inject(({ auth, setup }) => {
  const { customNames } = auth.settingsStore;
  const { addUsers } = setup.headerAction;

  return {
    addUsers,
    groupsCaption: customNames.groupsCaption,
  };
})(withRouter(withTranslation("Settings")(observer(SectionHeaderContent))));
