import React from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import i18n from "./i18n";
import AdvancedSelector from "../AdvancedSelector";
import { getUserList } from "../../api/people";
import { getGroupList } from "../../api/groups";
import Filter from "../../api/people/filter";
import UserTooltip from "./sub-components/UserTooltip";

class PeopleSelector extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      options: [],
      groups: [],
      page: 0,
      hasNextPage: true,
      isNextPageLoading: false
    };
  }

  componentDidMount() {
    const { language } = this.props;
    i18n.changeLanguage(language);

    getGroupList(this.props.useFake)
      .then(groups =>
        this.setState({
          groups: [
            {
              key: "all",
              label: "All groups",
              total: 0
            }
          ].concat(this.convertGroups(groups))
        })
      )
      .catch(error => console.log(error));
  }

  componentDidUpdate(prevProps) {
    if (this.props.language !== prevProps.language) {
      i18n.changeLanguage(this.props.language);
    }
  }

  convertGroups = groups => {
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

  convertUsers = users => {
    return users
      ? users.map(u => {
          return {
            key: u.id,
            groups: u.groups && u.groups.length ? u.groups.map(g => g.id) : [],
            label: u.displayName,
            email: u.email,
            position: u.title,
            avatarUrl: u.avatar
          };
        })
      : [];
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

      getUserList(filter, useFake)
        .then(response => {
          const newOptions = (startIndex ? [...this.state.options] : []).concat(
            this.convertUsers(response.items)
          );

          this.setState({
            hasNextPage: newOptions.length < response.total,
            isNextPageLoading: false,
            options: newOptions
          });
        })
        .catch(error => console.log(error));
    });
  };

  getOptionTooltipContent = index => {
    if (!index) return null;

    const { options } = this.state;

    const user = options[+index];

    if (!user) return null;

    // console.log("onOptionTooltipShow", index, user);

    return (
      <UserTooltip avatarUrl={user.avatarUrl} label={user.label} email={user.email} position={user.position} />
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
      selectedOptions,
      selectedGroups,
      hasNextPage,
      isNextPageLoading
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
      t
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
        displayType={"auto"}
        selectedOptions={selectedOptions}
        selectedGroups={selectedGroups}
        isOpen={isOpen}
        isMultiSelect={isMultiSelect}
        isDisabled={isDisabled}
        searchPlaceHolderLabel={t("SearchUsersPlaceholder")}
        selectButtonLabel={t("AddMembersButtonLabel")}
        selectAllLabel={t("SelectAllLabel")}
        groupsHeaderLabel={t("CustomDepartments", { departments: "Groups" })} //TODO: Replace to variable from settings
        emptySearchOptionsLabel={t("EmptySearchUsersResult")}
        emptyOptionsLabel={t("EmptyUsers")}
        loadingLabel={t("LoadingLabel")}
        onSelect={onSelect}
        onSearchChanged={this.onSearchChanged}
        onGroupChanged={this.onGroupChanged}
        getOptionTooltipContent={this.getOptionTooltipContent}
        onCancel={onCancel}
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
  size: PropTypes.oneOf(["full", "compact"]),
  language: PropTypes.string,
  t: PropTypes.func,
  role: PropTypes.oneOf(["admin", "user", "guest"])
};

PeopleSelector.defaultProps = {
  useFake: false,
  size: "full",
  language: "en",
  role: null
};

const ExtendedPeopleSelector = withTranslation()(PeopleSelector);

const PeopleSelectorWithI18n = props => {
  const { language } = props;
  i18n.changeLanguage(language);

  return <ExtendedPeopleSelector i18n={i18n} {...props} />;
};

PeopleSelectorWithI18n.propTypes = {
  language: PropTypes.string
};

function mapStateToProps(state) {
  return {
    language:
      state.auth &&
      ((state.auth.user && state.auth.user.cultureName) ||
        (state.auth.settings && state.auth.settings.culture))
  };
}

export default connect(mapStateToProps)(PeopleSelectorWithI18n);
