import React from "react";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import IconButton from "@docspace/components/icon-button";
import Headline from "@docspace/common/components/Headline";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";

import withLoader from "../../../../HOCs/withLoader";
import Loaders from "@docspace/common/components/Loaders";

const Wrapper = styled.div`
  display: grid;
  grid-template-columns: auto 1fr auto auto;
  align-items: center;

  .arrow-button {
    margin-right: 16px;

    @media (max-width: 1024px) {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }
`;

class SectionHeaderContent extends React.Component {
  constructor(props) {
    super(props);
  }

  onClickBack = () => {
    const { filter, resetGroup, setFilter, history } = this.props;

    resetGroup();
    history.goBack();
    setFilter(filter);
  };

  render() {
    const { group, t, groupCaption } = this.props;
    const headerText = group
      ? group.name
      : t("CustomNewDepartment", { groupCaption });

    return (
      <Wrapper>
        <IconButton
          iconName="/static/images/arrow.path.react.svg"
          size="17"
          isFill={true}
          onClick={this.onClickBack}
          className="arrow-button"
        />
        <Headline type="content" truncate={true}>
          {headerText}
        </Headline>
      </Wrapper>
    );
  }
}

SectionHeaderContent.propTypes = {
  group: PropTypes.object,
  history: PropTypes.object.isRequired,
};

SectionHeaderContent.defaultProps = {
  group: null,
};

export default withRouter(
  inject(({ auth, peopleStore }) => ({
    groupCaption: auth.settingsStore.customNames.groupCaption,
    filter: peopleStore.filterStore.filter,
    setFilter: peopleStore.filterStore.setFilterParams,
    group: peopleStore.selectedGroupStore.targetedGroup,
    resetGroup: peopleStore.selectedGroupStore.resetGroup,
  }))(
    observer(
      withTranslation("GroupAction")(
        withLoader(SectionHeaderContent)(<Loaders.SectionHeader />)
      )
    )
  )
);
