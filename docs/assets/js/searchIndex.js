
var camelCaseTokenizer = function (builder) {

  var pipelineFunction = function (token) {
    var previous = '';
    // split camelCaseString to on each word and combined words
    // e.g. camelCaseTokenizer -> ['camel', 'case', 'camelcase', 'tokenizer', 'camelcasetokenizer']
    var tokenStrings = token.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
      var current = cur.toLowerCase();
      if (acc.length === 0) {
        previous = current;
        return acc.concat(current);
      }
      previous = previous.concat(current);
      return acc.concat([current, previous]);
    }, []);

    // return token for each string
    // will copy any metadata on input token
    return tokenStrings.map(function(tokenString) {
      return token.clone(function(str) {
        return tokenString;
      })
    });
  }

  lunr.Pipeline.registerFunction(pipelineFunction, 'camelCaseTokenizer')

  builder.pipeline.before(lunr.stemmer, pipelineFunction)
}
var searchModule = function() {
    var documents = [];
    var idMap = [];
    function a(a,b) { 
        documents.push(a);
        idMap.push(b); 
    }

    a(
        {
            id:0,
            title:"IndexKeywordGroup",
            content:"IndexKeywordGroup",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IndexKeywordGroup',
            title:"IndexKeywordGroup",
            description:""
        }
    );
    a(
        {
            id:1,
            title:"IdpfPackage",
            content:"IdpfPackage",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IdpfPackage',
            title:"IdpfPackage",
            description:""
        }
    );
    a(
        {
            id:2,
            title:"PublicationContext",
            content:"PublicationContext",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications/PublicationContext',
            title:"PublicationContext",
            description:""
        }
    );
    a(
        {
            id:3,
            title:"PublicationNamespaces",
            content:"PublicationNamespaces",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/PublicationNamespaces',
            title:"PublicationNamespaces",
            description:""
        }
    );
    a(
        {
            id:4,
            title:"MarkdownPresentationCommands",
            content:"MarkdownPresentationCommands",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/MarkdownPresentationCommands',
            title:"MarkdownPresentationCommands",
            description:""
        }
    );
    a(
        {
            id:5,
            title:"OebpsTextToc",
            content:"OebpsTextToc",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/OebpsTextToc',
            title:"OebpsTextToc",
            description:""
        }
    );
    a(
        {
            id:6,
            title:"PublicationContextExtensions",
            content:"PublicationContextExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/PublicationContextExtensions',
            title:"PublicationContextExtensions",
            description:""
        }
    );
    a(
        {
            id:7,
            title:"ISegment",
            content:"ISegment",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/ISegment',
            title:"ISegment",
            description:""
        }
    );
    a(
        {
            id:8,
            title:"OebpsTextBiography",
            content:"OebpsTextBiography",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/OebpsTextBiography',
            title:"OebpsTextBiography",
            description:""
        }
    );
    a(
        {
            id:9,
            title:"IDocumentExtensions",
            content:"IDocumentExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/IDocumentExtensions',
            title:"IDocumentExtensions",
            description:""
        }
    );
    a(
        {
            id:10,
            title:"OebpsTextDedication",
            content:"OebpsTextDedication",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/OebpsTextDedication',
            title:"OebpsTextDedication",
            description:""
        }
    );
    a(
        {
            id:11,
            title:"MarkdownEntryActivity",
            content:"MarkdownEntryActivity",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Activities/MarkdownEntryActivity',
            title:"MarkdownEntryActivity",
            description:""
        }
    );
    a(
        {
            id:12,
            title:"DirectoryInfoExtensions",
            content:"DirectoryInfoExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/DirectoryInfoExtensions',
            title:"DirectoryInfoExtensions",
            description:""
        }
    );
    a(
        {
            id:13,
            title:"ImageCandidate",
            content:"ImageCandidate",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/ImageCandidate',
            title:"ImageCandidate",
            description:""
        }
    );
    a(
        {
            id:14,
            title:"ISegmentExtensions",
            content:"ISegmentExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/ISegmentExtensions',
            title:"ISegmentExtensions",
            description:""
        }
    );
    a(
        {
            id:15,
            title:"ImageSize",
            content:"ImageSize",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/ImageSize',
            title:"ImageSize",
            description:""
        }
    );
    a(
        {
            id:16,
            title:"IIndexKeyword",
            content:"IIndexKeyword",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IIndexKeyword',
            title:"IIndexKeyword",
            description:""
        }
    );
    a(
        {
            id:17,
            title:"Fragment",
            content:"Fragment",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/Fragment',
            title:"Fragment",
            description:""
        }
    );
    a(
        {
            id:18,
            title:"MarkdownEntry",
            content:"MarkdownEntry",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/MarkdownEntry',
            title:"MarkdownEntry",
            description:""
        }
    );
    a(
        {
            id:19,
            title:"DaisyConsortiumNcx",
            content:"DaisyConsortiumNcx",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/DaisyConsortiumNcx',
            title:"DaisyConsortiumNcx",
            description:""
        }
    );
    a(
        {
            id:20,
            title:"PublicationsActivitiesGetter",
            content:"PublicationsActivitiesGetter",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Activities/PublicationsActivitiesGetter',
            title:"PublicationsActivitiesGetter",
            description:""
        }
    );
    a(
        {
            id:21,
            title:"IGroupableExtensions",
            content:"IGroupableExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/IGroupableExtensions',
            title:"IGroupableExtensions",
            description:""
        }
    );
    a(
        {
            id:22,
            title:"JObjectExtensions",
            content:"JObjectExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/JObjectExtensions',
            title:"JObjectExtensions",
            description:""
        }
    );
    a(
        {
            id:23,
            title:"ResponsiveImageExtensions",
            content:"ResponsiveImageExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/ResponsiveImageExtensions',
            title:"ResponsiveImageExtensions",
            description:""
        }
    );
    a(
        {
            id:24,
            title:"IndexCommands",
            content:"IndexCommands",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IndexCommands',
            title:"IndexCommands",
            description:""
        }
    );
    a(
        {
            id:25,
            title:"PublicationFiles",
            content:"PublicationFiles",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/PublicationFiles',
            title:"PublicationFiles",
            description:""
        }
    );
    a(
        {
            id:26,
            title:"MarkdownPresentationDirectories",
            content:"MarkdownPresentationDirectories",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/MarkdownPresentationDirectories',
            title:"MarkdownPresentationDirectories",
            description:""
        }
    );
    a(
        {
            id:27,
            title:"ResponsiveImage",
            content:"ResponsiveImage",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/ResponsiveImage',
            title:"ResponsiveImage",
            description:""
        }
    );
    a(
        {
            id:28,
            title:"MarkdownEntryUtility",
            content:"MarkdownEntryUtility",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications/MarkdownEntryUtility',
            title:"MarkdownEntryUtility",
            description:""
        }
    );
    a(
        {
            id:29,
            title:"ProgramArgsExtensions",
            content:"ProgramArgsExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/ProgramArgsExtensions',
            title:"ProgramArgsExtensions",
            description:""
        }
    );
    a(
        {
            id:30,
            title:"PublicationChapter",
            content:"PublicationChapter",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/PublicationChapter',
            title:"PublicationChapter",
            description:""
        }
    );
    a(
        {
            id:31,
            title:"IIndexKeywordGroup",
            content:"IIndexKeywordGroup",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IIndexKeywordGroup',
            title:"IIndexKeywordGroup",
            description:""
        }
    );
    a(
        {
            id:32,
            title:"EpubUtility",
            content:"EpubUtility",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications/EpubUtility',
            title:"EpubUtility",
            description:""
        }
    );
    a(
        {
            id:33,
            title:"Segment",
            content:"Segment",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/Segment',
            title:"Segment",
            description:""
        }
    );
    a(
        {
            id:34,
            title:"IndexKeyword",
            content:"IndexKeyword",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IndexKeyword',
            title:"IndexKeyword",
            description:""
        }
    );
    a(
        {
            id:35,
            title:"Document",
            content:"Document",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/Document',
            title:"Document",
            description:""
        }
    );
    a(
        {
            id:36,
            title:"IDocument",
            content:"IDocument",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IDocument',
            title:"IDocument",
            description:""
        }
    );
    a(
        {
            id:37,
            title:"MarkdownUtility",
            content:"MarkdownUtility",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications/MarkdownUtility',
            title:"MarkdownUtility",
            description:""
        }
    );
    a(
        {
            id:38,
            title:"CloneInitializers",
            content:"CloneInitializers",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/CloneInitializers',
            title:"CloneInitializers",
            description:""
        }
    );
    a(
        {
            id:39,
            title:"PublicationAppScalars",
            content:"PublicationAppScalars",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/PublicationAppScalars',
            title:"PublicationAppScalars",
            description:""
        }
    );
    a(
        {
            id:40,
            title:"MarkdownEntryExtensions",
            content:"MarkdownEntryExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/MarkdownEntryExtensions',
            title:"MarkdownEntryExtensions",
            description:""
        }
    );
    a(
        {
            id:41,
            title:"SearchIndexActivity",
            content:"SearchIndexActivity",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Activities/SearchIndexActivity',
            title:"SearchIndexActivity",
            description:""
        }
    );
    a(
        {
            id:42,
            title:"IFragmentExtensions",
            content:"IFragmentExtensions",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Extensions/IFragmentExtensions',
            title:"IFragmentExtensions",
            description:""
        }
    );
    a(
        {
            id:43,
            title:"OebpsTextCopyright",
            content:"OebpsTextCopyright",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/OebpsTextCopyright',
            title:"OebpsTextCopyright",
            description:""
        }
    );
    a(
        {
            id:44,
            title:"IFragment",
            content:"IFragment",
            description:'',
            tags:''
        },
        {
            url:'/Songhay.Publications/api/Songhay.Publications.Models/IFragment',
            title:"IFragment",
            description:""
        }
    );
    var idx = lunr(function() {
        this.field('title');
        this.field('content');
        this.field('description');
        this.field('tags');
        this.ref('id');
        this.use(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
        documents.forEach(function (doc) { this.add(doc) }, this)
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
