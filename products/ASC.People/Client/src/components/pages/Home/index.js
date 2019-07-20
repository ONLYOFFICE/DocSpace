import React, { useState } from "react";
import { connect } from "react-redux";
import PropTypes from "prop-types";
import { withRouter } from "react-router";
import _ from "lodash";
import { PageLayout } from "asc-web-components";
import {ArticleHeaderContent, ArticleBodyContent, ArticleMainButtonContent} from '../../Article';
import {SectionHeaderContent, SectionBodyContent, SectionFilterContent, SectionPagingContent} from './Section';

let selection = [];

const Home = ({users}) => {
  const [isHeaderVisible, toggleHeaderVisible] = useState(false);
  const [isHeaderIndeterminate, toggleHeaderIndeterminate] = useState(false);
  const [isHeaderChecked, toggleHeaderChecked] = useState(false);
  
  const renderGroupButtonMenu = () => {
    const headerVisible = selection.length > 0;
    const headerIndeterminate = headerVisible && selection.length > 0 && selection.length < users.length;
    const headerChecked = headerVisible && selection.length === users.length;
    
    /*console.log(`renderGroupButtonMenu()
      headerVisible=${headerVisible} 
      headerIndeterminate=${headerIndeterminate} 
      headerChecked=${headerChecked}
      selection.length=${selection.length}
      users.length=${users.length}`);*/

    if(headerVisible)
      toggleHeaderVisible(headerVisible);

    if(isHeaderIndeterminate !== headerIndeterminate)
      toggleHeaderIndeterminate(headerIndeterminate);

    if(isHeaderChecked !== headerChecked)
      toggleHeaderChecked(headerChecked);
  };

  const onRowSelect = (checked, data) => {
    /*console.log(`onBodySelect 
        row.checked=${checked}`,
          data,
          selection);*/

    const id = _.result(
      _.find(selection, function(obj) {
        return obj.id === data.id;
      }),
      "id"
    );
    if (checked && !id) {
      selection.push(data);
    } else if (id) {
      selection = _.filter(selection, function(obj) {
        return obj.id !== id;
      });
    }

    renderGroupButtonMenu();
  };

  return (
    <PageLayout
      articleHeaderContent={<ArticleHeaderContent />}
      articleMainButtonContent={<ArticleMainButtonContent />}
      articleBodyContent={<ArticleBodyContent />}
      sectionHeaderContent={
        <SectionHeaderContent
          isHeaderVisible={isHeaderVisible}
          isHeaderIndeterminate={isHeaderIndeterminate}
          isHeaderChecked={isHeaderChecked}
          onCheck={checked => {
            toggleHeaderChecked(checked);
            selection = checked ? [...users] : [];
            /*console.log(`SectionHeaderContent onCheck 
                selection.length=${selection.length}`)*/
            renderGroupButtonMenu();
          }}
        />
      }
      sectionFilterContent={<SectionFilterContent />}
      sectionBodyContent={
        <SectionBodyContent
          users={users}
          isHeaderChecked={isHeaderChecked}
          onSelect={onRowSelect}
        />
      }
      sectionPagingContent={<SectionPagingContent />}
    />
  );
};

Home.propTypes = {
  users: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
    users: state.people.users,
    isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(Home));