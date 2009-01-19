﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.MappingModel.Collections;
using NHibernate.Cfg.MappingSchema;

namespace FluentNHibernate.MappingModel.Output
{
    public class HbmSetWriter : MappingModelVisitorBase, IHbmWriter<SetMapping>
    {
        private readonly IHbmWriter<ICollectionContentsMapping> _contentsWriter;
        private readonly IHbmWriter<KeyMapping> _keyWriter;

        private HbmSet _hbmSet;

        public HbmSetWriter(IHbmWriter<ICollectionContentsMapping> contentsWriter, IHbmWriter<KeyMapping> keyWriter)
        {
            _contentsWriter = contentsWriter;
            _keyWriter = keyWriter;
        }

        public object Write(SetMapping mappingModel)
        {
            mappingModel.AcceptVisitor(this);
            return _hbmSet;
        }

        public override void ProcessSet(SetMapping setMapping)
        {
            _hbmSet = new HbmSet();
            _hbmSet.name = setMapping.Name;

            if(setMapping.Attributes.IsSpecified(x => x.OrderBy))
                _hbmSet.orderby = setMapping.OrderBy;

            if (setMapping.Attributes.IsSpecified(x => x.IsInverse))
                _hbmSet.inverse = setMapping.IsInverse;

            if (setMapping.Attributes.IsSpecified(x => x.IsLazy))
            {
                _hbmSet.lazy = setMapping.IsLazy;
                _hbmSet.lazySpecified = true;
            }
        }

        public override void ProcessCollectionContents(ICollectionContentsMapping contentsMapping)
        {
            object contentsHbm = _contentsWriter.Write(contentsMapping);
            _hbmSet.Item1 = contentsHbm;
        }

        public override void ProcessKey(KeyMapping keyMapping)
        {
            HbmKey keyHbm = (HbmKey)_keyWriter.Write(keyMapping);
            _hbmSet.key = keyHbm;
        }
    }
}
