import React, { useEffect } from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "../AdvancedSelector";
import { getUserList } from "../../api/people";
import { getGroupList } from "../../api/groups";
import Filter from "../../api/people/filter";
import UserTooltip from "./sub-components/UserTooltip";
import { changeLanguage } from "../../utils";

class PeopleSelector extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      options: [],
      groups: [],
      page: 0,
      hasNextPage: true,
      isNextPageLoading: false,
    };
  }

  componentDidMount() {
    const { groupsCaption } = this.props;
    changeLanguage(i18n).then((t) =>
      getGroupList(this.props.useFake)
        .then((groups) =>
          this.setState({
            groups: [
              {
                key: "all",
                label: t("CustomAllGroups", { groupsCaption }),
                total: 0,
              },
            ].concat(this.convertGroups(groups)),
          })
        )
        .catch((error) => console.log(error))
    );
  }

  componentDidUpdate(prevProps) {
    if (this.props.language !== prevProps.language) {
      i18n.changeLanguage(this.props.language);
    }
  }

  convertGroups = (groups) => {
    return groups
      ? groups.map((g) => {
          return {
            key: g.id,
            label: g.name,
            total: 0,
          };
        })
      : [];
  };

  convertUser = (u) => {
    return {
      key: u.id,
      groups: u.groups && u.groups.length ? u.groups.map((g) => g.id) : [],
      label: u.displayName,
      email: u.email,
      position: u.title,
      avatarUrl: u.avatar,
    };
  };

  convertUsers = (users) => {
    return users ? users.map(this.convertUser) : [];
  };

  loadNextPage = ({ startIndex, searchValue, currentGroup }) => {
    console.log(
      `loadNextPage(startIndex=${startIndex}, searchValue="${searchValue}", currentGroup="${currentGroup}")`
    );

    const pageCount = 100;

    this.setState({ isNextPageLoading: true }, () => {
      const { role, useFake } = this.props;

      const filter = Filter.getDefault();
      filter.page = startIndex / pageCount;
      filter.pageCount = pageCount;

      if (searchValue) {
        filter.search = searchValue;
      }

      if (role) {
        filter.role = role;
      }

      if (currentGroup && currentGroup !== "all") filter.group = currentGroup;

      const { defaultOption, defaultOptionLabel } = this.props;

      getUserList(filter, useFake)
        .then((response) => {
          let newOptions = startIndex ? [...this.state.options] : [];

          if (defaultOption) {
            const inGroup =
              !currentGroup ||
              currentGroup === "all" ||
              (defaultOption.groups &&
                defaultOption.groups.filter((g) => g.id === currentGroup)
                  .length > 0);

            if (searchValue) {
              const exists = response.items.find(
                (item) => item.id === defaultOption.id
              );

              if (exists && inGroup) {
                newOptions.push(
                  this.convertUser({
                    ...defaultOption,
                    displayName: defaultOptionLabel,
                  })
                );
              }
            } else if (!startIndex && response.items.length > 0 && inGroup) {
              newOptions.push(
                this.convertUser({
                  ...defaultOption,
                  displayName: defaultOptionLabel,
                })
              );
            }

            newOptions = newOptions.concat(
              this.convertUsers(
                response.items.filter((item) => item.id !== defaultOption.id)
              )
            );
          } else {
            newOptions = newOptions.concat(this.convertUsers(response.items));
          }

          this.setState({
            hasNextPage: newOptions.length < response.total,
            isNextPageLoading: false,
            options: newOptions,
          });
        })
        .catch((error) => console.log(error));
    });
  };

  getOptionTooltipContent = (index) => {
    if (!index) return null;

    const { options } = this.state;

    const user = options[+index];

    if (!user) return null;

    // console.log("onOptionTooltipShow", index, user);

    const { defaultOption } = this.props;

    const label =
      defaultOption && defaultOption.id === user.key
        ? defaultOption.displayName
        : user.label;

    return (
      <UserTooltip
        avatarUrl={user.avatarUrl}
        label={label}
        email={user.email}
        position={user.position}
      />
    );
  };

  onSearchChanged = () => {
    //console.log("onSearchChanged")(value);
    this.setState({ options: [], hasNextPage: true });
  };

  onGroupChanged = () => {
    //console.log("onGroupChanged")(group);
    this.setState({ options: [], hasNextPage: true });
  };

  render() {
    const {
      options,
      groups,
      selectedGroups,
      hasNextPage,
      isNextPageLoading,
    } = this.state;

    const {
      id,
      className,
      style,
      isOpen,
      isMultiSelect,
      isDisabled,
      onSelect,
      size,
      onCancel,
      t,
      searchPlaceHolderLabel,
      groupsCaption,
      displayType,
      withoutAside,
      embeddedComponent,
      selectedOptions,
      showCounter,
    } = this.props;

    return (
      <AdvancedSelector
        id={id}
        className={className}
        style={style}
        options={options}
        groups={groups}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadNextPage={this.loadNextPage}
        size={size}
        displayType={displayType}
        selectedOptions={selectedOptions}
        selectedGroups={selectedGroups}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={
          searchPlaceHolderLabel || t("SearchUsersPlaceholder")
        }
        selectButtonLabel={t("AddMembersButtonLabel")}
        selectAllLabel={t("SelectAllLabel")}
        groupsHeaderLabel={groupsCaption}
        emptySearchOptionsLabel={t("EmptySearchUsersResult")}
        emptyOptionsLabel={t("EmptyUsers")}
        loadingLabel={t("LoadingLabel")}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onGroupChanged={this.onGroupChanged}
        //getOptionTooltipContent={this.getOptionTooltipContent}
        onCancel={onCancel}
        withoutAside={withoutAside}
        embeddedComponent={embeddedComponent}
        showCounter={showCounter}
      />
    );
  }
}

PeopleSelector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOf([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,
  isOpen: PropTypes.bool,
  onSelect: PropTypes.func,
  onCancel: PropTypes.func,
  useFake: PropTypes.bool,
  isMultiSelect: PropTypes.bool,
  isDisabled: PropTypes.bool,
  defaultOption: PropTypes.object,
  defaultOptionLabel: PropTypes.string,
  size: PropTypes.oneOf(["full", "compact"]),
  language: PropTypes.string,
  t: PropTypes.func,
  groupsCaption: PropTypes.string,
  searchPlaceHolderLabel: PropTypes.string,
  role: PropTypes.oneOf(["admin", "user", "guest"]),
  displayType: PropTypes.oneOf(["auto", "aside", "dropdown"]),
  withoutAside: PropTypes.bool,
  embeddedComponent: PropTypes.any,
};

PeopleSelector.defaultProps = {
  useFake: false,
  size: "full",
  language: "en",
  role: null,
  defaultOption: null,
  defaultOptionLabel: "Me",
  groupsCaption: "Groups",
  displayType: "auto",
  withoutAside: false,
};

const ExtendedPeopleSelector = withTranslation()(PeopleSelector);

const PeopleSelectorWithI18n = (props) => {
  useEffect(() => {
    changeLanguage(i18n);
  }, []);

  return <ExtendedPeopleSelector i18n={i18n} {...props} />;
};

PeopleSelectorWithI18n.propTypes = {
  language: PropTypes.string,
};

export default PeopleSelectorWithI18n;
