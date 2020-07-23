import React from "react";
import styled from "styled-components";
import { withRouter } from "react-router";
import { Headline } from 'asc-web-common';
import { IconButton, utils } from "asc-web-components";
import { withTranslation } from 'react-i18next';
import { getKeyByLink, settingsTree, getTKeyByKey } from '../../../utils';

const { tablet } = utils.device;

const HeaderContainer = styled.div`
  position: relative;
    display: flex;
    align-items: center;
    max-width: calc(100vw - 32px);

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
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    this.state = {
      header
    };

  }

  componentDidUpdate() {
    const { match, location } = this.props;
    const fullSettingsUrl = match.url;
    const locationPathname = location.pathname;


    const fullSettingsUrlLength = fullSettingsUrl.length;
    const resultPath = locationPathname.slice(fullSettingsUrlLength + 1);
    const arrayOfParams = resultPath.split('/');

    const key = getKeyByLink(arrayOfParams, settingsTree);
    const header = getTKeyByKey(key, settingsTree);
    header !== this.state.header && this.setState({ header });

  }

  onBackToParent = () => {
    console.log("clickToParent")
  }

  render() {
    const { t } = this.props;
    const { header } = this.state;

    return (
      <HeaderContainer>
        {true && (
          <IconButton
            iconName="ArrowPathIcon"
            size="17"
            color="#A3A9AE"
            hoverColor="#657077"
            isFill={true}
            onClick={this.onBackToParent}
            className="arrow-button"
          />
        )}
        <Headline type='content' truncate={true}>
          {t(header)}
        </Headline>
      </HeaderContainer>
    );
  }
};

export default withRouter(withTranslation()(SectionHeaderContent));
