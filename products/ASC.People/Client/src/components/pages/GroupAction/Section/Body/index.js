import React from "react";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router";
import PropTypes from "prop-types";
import {
  Button,
  TextInput,
  Icons,
  SelectedItem,
  AdvancedSelector,
  FieldContainer,
  ComboBox,
  ComboButton,
  ModalDialog,
  SearchInput,
  toastr,
  utils
} from "asc-web-components";
import {
  department,
  headOfDepartment,
  typeUser
} from "../../../../../helpers/customNames";
import { connect } from "react-redux";
import {
  resetGroup,
  createGroup,
  updateGroup
} from "../../../../../store/group/actions";
import styled from "styled-components";
import { fetchSelectorUsers } from "../../../../../store/people/actions";
import { GUID_EMPTY } from "../../../../../helpers/constants";
import isEqual from "lodash/isEqual";

const MainContainer = styled.div`
  display: flex;
  flex-direction: column;

  .group-name_container {
    width: 320px;
  }

  .head_container {
    position: relative;
    width: 320px;
  }

  .members_container {
    position: relative;
    width: 320px;
  }

  .search_container {
    margin-top: 16px;
  }

  .selected-members_container {
    margin-top: 16px;
    display: flex;
    flex-wrap: wrap;
    flex-direction: row;

    .selected-item {
      margin-right: 8px;
      margin-bottom: 8px;
    }
  }

  .buttons_container {
    margin-top: 60px;

    .cancel-button {
      margin-left: 8px;
    }
  }

  @media ${utils.device.tablet} {
    .search_container {
      width: 320px;
    }
  }
`;

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);
    this.state = this.mapPropsToState();
  }

  mapPropsToState = () => {
    const { group, users, groups, t } = this.props;

    const newState = {
      id: group ? group.id : "",
      groupName: group ? group.name : "",
      searchValue: "",
      error: null,
      inLoading: false,
      isHeaderSelectorOpen: false,
      isUsersSelectorOpen: false,
      users: users,
      groups: groups,
      modalVisible: false,
      header: group
        ? {
            key: 0,
            label: "{SELECTED HEADER NAME}" //group.head
          }
        : {
            key: 0,
            label: t("CustomAddEmployee", { typeUser })
          },
      groupMembers: group && group.members ? group.members.map(m => {
        return {
          key: m.id,
          label: m.displayName
        }
      }) : [],
      groupManager: group && group.manager ? {
        key: group.manager.id,
        label: group.manager.displayName
      } : {
        key: GUID_EMPTY,
        label: t("CustomAddEmployee", { typeUser })
      }
    };

    return newState;
  }

  componentDidMount() {
    const { users, fetchSelectorUsers } = this.props;
    if(!users || !users.length) {
      fetchSelectorUsers();
    }
  }

  componentDidUpdate(prevProps) {
    //const { users, group } = this.props;
    if(!isEqual(this.props, prevProps)) {
      this.setState(this.mapPropsToState());
    }
  }

  onGroupChange = e => {
    this.setState({
      groupName: e.target.value
    });
  };

  onSearchChange = value => {
    this.setState({
      searchValue: value
    });
  };

  onHeadSelectorSearch = value => {
    /*setOptions(
      options.filter(option => {
        return option.label.indexOf(value) > -1;
      })
    );*/
  };

  onHeadSelectorSelect = option => {
    this.setState({ 
      groupManager: {
        key: option.key,
        label: option.label
      },
      isHeaderSelectorOpen: !this.state.isHeaderSelectorOpen 
    });
  };

  onHeadSelectorClick = () => {
    this.setState({
      isHeaderSelectorOpen: !this.state.isHeaderSelectorOpen
    });
  };

  onUsersSelectorSearch = (value) => {
    /*setOptions(
      options.filter(option => {
        return option.label.indexOf(value) > -1;
      })
    );*/
  };
  onUsersSelectorSelect = (selectedOptions) => {
    //console.log("onSelect", selectedOptions);
    //this.onUsersSelectorClick();
    this.setState({ 
      groupMembers: selectedOptions.map(option => {
        return {
          key: option.key,
          label: option.label
        };
      }),
      isUsersSelectorOpen: !this.state.isUsersSelectorOpen 
    });
  };

  onUsersSelectorClick = () => {
    this.setState({
      isUsersSelectorOpen: !this.state.isUsersSelectorOpen
    });
  };

  toggleModalVisible = () => {
    this.setState({
      modalVisible: !this.state.modalVisible
    });
  };

  onSave = () => {
    const { history, group, createGroup, updateGroup, resetGroup } = this.props;
    const { groupName, groupManager, groupMembers } = this.state;

    if (!groupName || !groupName.trim().length) return false;

    this.setState({ inLoading: true });

    (group && group.id
      ? updateGroup(
          group.id,
          groupName,
          groupManager.key,
          groupMembers.map(u => u.key)
        )
      : createGroup(groupName, groupManager.key, groupMembers.map(u => u.key))
    )
      .then(() => {
        toastr.success("Success");
        //this.setState({ inLoading: true });
        history.goBack();
        resetGroup();
      })
      .catch(error => {
        toastr.error(error.message);
        this.setState({ inLoading: false });
      });
  };

  onCancel = () => {
    const { history, resetGroup } = this.props;

    resetGroup();
    history.goBack();
  };

  onSelectedItemClose = (member) => {
    this.setState({ 
      groupMembers: this.state.groupMembers.filter(g => g.key !== member.key)
    });
  }

  renderModal = () => {
    const { groups, modalVisible } = this.state;

    return (
      <ModalDialog
        zIndex={1001}
        visible={modalVisible}
        headerContent="New User"
        bodyContent={
          <div className="create_new_user_modal">
            <FieldContainer
              isVertical={true}
              isRequired={true}
              hasError={false}
              labelText={"First name:"}
            >
              <TextInput
                value={""}
                hasError={false}
                className="firstName-input"
                scale={true}
                autoComplete="off"
                onChange={e => {
                  //set(e.target.value);
                }}
              />
            </FieldContainer>
            <FieldContainer
              isVertical={true}
              isRequired={true}
              hasError={false}
              labelText={"Last name:"}
            >
              <TextInput
                value={""}
                hasError={false}
                className="lastName-input"
                scale={true}
                autoComplete="off"
                onChange={e => {
                  //set(e.target.value);
                }}
              />
            </FieldContainer>
            <FieldContainer
              isVertical={true}
              isRequired={true}
              hasError={false}
              labelText={"E-mail:"}
            >
              <TextInput
                value={""}
                hasError={false}
                className="email-input"
                scale={true}
                autoComplete="off"
                onChange={e => {
                  //set(e.target.value);
                }}
              />
            </FieldContainer>
            <FieldContainer
              isVertical={true}
              isRequired={true}
              hasError={false}
              labelText={"Group:"}
            >
              <ComboBox
                options={groups}
                className="group-input"
                onSelect={option => console.log("Selected option", option)}
                selectedOption={{
                  key: 0,
                  label: "Select"
                }}
                dropDownMaxHeight={200}
                scaled={true}
                scaledOptions={true}
                size="content"
              />
            </FieldContainer>
          </div>
        }
        footerContent={[
          <Button
            key="CreateBtn"
            label="Create"
            primary={true}
            size="big"
            onClick={e => {
              console.log("CreateBtn click", e);
              this.toggleModalVisible();
            }}
          />
        ]}
        onClose={this.toggleModalVisible}
      />
    );
  };

  render() {
    const { t } = this.props;
    const {
      groupName,
      users,
      groups,
      groupMembers,
      isHeaderSelectorOpen: isHeadSelectorOpen,
      isUsersSelectorOpen,
      inLoading,
      error,
      searchValue,
      modalVisible,
      groupManager
    } = this.state;
    return (
      <MainContainer>
        <div style={{ visibility: "hidden", width: 1, height: 1 }}>
          <Icons.SearchIcon size="small" />
        </div>
        <FieldContainer
          className="group-name_container"
          isRequired={true}
          hasError={false}
          isVertical={true}
          labelText={t("CustomDepartmentName", { department })}
        >
          <TextInput
            id="group-name"
            name="group-name"
            scale={true}
            isAutoFocussed={true}
            tabIndex={1}
            value={groupName}
            onChange={this.onGroupChange}
            isDisabled={inLoading}
          />
        </FieldContainer>
        <FieldContainer
          className="head_container"
          isRequired={false}
          hasError={false}
          isVertical={true}
          labelText={t("CustomHeadOfDepartment", { headOfDepartment })}
        >
          <ComboButton
            id="head-selector"
            tabIndex={2}
            options={[]}
            isOpen={isHeadSelectorOpen}
            selectedOption={groupManager}
            scaled={true}
            isDisabled={inLoading}
            size="content"
            opened={isHeadSelectorOpen}
            onClick={this.onHeadSelectorClick}
          >
            <Icons.CatalogGuestIcon size="medium" />
          </ComboButton>
          <AdvancedSelector
            isDropDown={true}
            isOpen={isHeadSelectorOpen}
            size="full"
            placeholder={"Search"}
            onSearchChanged={this.onHeadSelectorSearch}
            options={users}
            groups={groups}
            isMultiSelect={false}
            buttonLabel={t("CustomAddEmployee", { typeUser })}
            selectAllLabel={"Select all"}
            onSelect={this.onHeadSelectorSelect}
            onCancel={this.onHeadSelectorClick}
            allowCreation={false}
            //onAddNewClick={toggleModalVisible}
            allowAnyClickClose={true}
          />
        </FieldContainer>
        <FieldContainer
          className="members_container"
          isRequired={false}
          hasError={false}
          isVertical={true}
          labelText="Members"
        >
          <ComboButton
            id="users-selector"
            tabIndex={3}
            options={[]}
            isOpen={isUsersSelectorOpen}
            isDisabled={inLoading}
            selectedOption={{
              key: 0,
              label: t("CustomAddEmployee", { typeUser })
            }}
            scaled={true}
            size="content"
            opened={isUsersSelectorOpen}
            onClick={this.onUsersSelectorClick}
          >
            <Icons.CatalogGuestIcon size="medium" />
          </ComboButton>
          <AdvancedSelector
            isDropDown={true}
            isOpen={isUsersSelectorOpen}
            size="full"
            placeholder={"Search"}
            onSearchChanged={this.onUsersSelectorSearch}
            options={users}
            groups={groups}
            isMultiSelect={true}
            buttonLabel={t("CustomAddEmployee", { typeUser })}
            selectAllLabel={"Select all"}
            onSelect={this.onUsersSelectorSelect}
            onCancel={this.onUsersSelectorClick}
            allowCreation={true}
            onAddNewClick={this.toggleModalVisible}
            allowAnyClickClose={!modalVisible}
          />
        </FieldContainer>
        {groupMembers && groupMembers.length > 0 && (
          <div className="search_container">
            <SearchInput
              id="member-search"
              isDisabled={inLoading}
              scale={true}
              placeholder="Search"
              value={searchValue}
              onChange={this.onSearchChange}
            />
          </div>
        )}
        <div className="selected-members_container">
          {groupMembers.map(member => (
            <SelectedItem
              key={member.key}
              text={member.label}
              onClose={this.onSelectedItemClose.bind(this, member)}
              isInline={true}
              className="selected-item"
              isDisabled={inLoading}
            />
          ))}
        </div>
        <div>{error && <strong>{error}</strong>}</div>
        <div className="buttons_container">
          <Button
            label={t("SaveButton")}
            primary
            type="submit"
            isLoading={inLoading}
            size="big"
            tabIndex={4}
            onClick={this.onSave}
          />
          <Button
            label={t("CancelButton")}
            className="cancel-button"
            size="big"
            isDisabled={inLoading}
            onClick={this.onCancel}
            tabIndex={5}
          />
        </div>
        {this.renderModal()}
      </MainContainer>
    );
  }
}

SectionBodyContent.propTypes = {
  group: PropTypes.object
};

SectionBodyContent.defaultProps = {
  group: null
};

const convertUsers = users => {
  return users
    ? users.map(u => {
        return {
          key: u.id,
          groups: u.groups || [],
          label: u.displayName
        };
      })
    : [];
};

const convertGroups = groups => {
  return groups
    ? groups.map(g => {
        return {
          key: g.id,
          label: g.name,
          total: 0
        };
      })
    : [];
};

function mapStateToProps(state) {
  return {
    settings: state.auth.settings,
    group: state.group.targetGroup,
    groups: convertGroups(state.people.groups),
    users: convertUsers(state.people.selector.users) //TODO: replace to api requests with search
  };
}

export default connect(
  mapStateToProps,
  { resetGroup, createGroup, updateGroup, fetchSelectorUsers }
)(withRouter(withTranslation()(SectionBodyContent)));
